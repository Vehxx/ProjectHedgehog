using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class RestaurantCustomerHandler : MonoBehaviour
{
    public GlobalCustomerHandler GCH;
    public TimeHandler TH;
    public DrinkManager DM;
    public Tilemap obstacleTilemap;  

    [SerializeField] private GameObject charModel;
    [SerializeField] private GameObject Grid;           // world parent (no canvas)
    [SerializeField] private Tilemap chairTilemap;      // seats
    [SerializeField] private Tilemap doorMap;           // exit

    [SerializeField] private bool enableRandomFlow = true;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float arriveThreshold = 0.05f;
    [SerializeField] private float hardMoveTimeout = 1000f;

    // NEW: gridlike stepping + obstacles
    [SerializeField] private bool gridlikeMovement = true;    // toggle grid stepping
    [SerializeField] private float stepInterval = 0.5f;         // one axis move per second

    public int lastTime = 0;
    public List<Customer> toSpawn = new();
    public List<Customer> current = new();
    public List<Customer> toLeave = new();

    // seat/state tracking (by tile cell, not Vector3)
    private readonly List<Vector3Int> chairCells = new();
    private readonly Dictionary<Vector3Int, bool> chairOccupied = new();    // cell -> occupied?
    private readonly Dictionary<Customer, Vector3Int> seatCellOf = new();   // customer -> cell
    private readonly Dictionary<Customer, GameObject> avatarOf = new();     // customer -> avatar
    private readonly Dictionary<Customer, Coroutine> moveRoutineOf = new(); // customer -> active move

    private Vector3 doorWorld;

    // ---- Utilities for grid movement ----
    private Tilemap RefGrid => chairTilemap ? chairTilemap : doorMap;

    private Vector3 CenterFromCell(Tilemap map, Vector3Int cell) =>
        map.CellToWorld(cell) + map.tileAnchor;

    private bool IsBlockedCell(Vector3Int cell)
    {
        return obstacleTilemap && obstacleTilemap.HasTile(cell);
    }

    // ---------- Unity ----------
    void Start()
    {
        if (!chairTilemap) Debug.LogError("[RCH] chairTilemap not assigned.");
        if (!doorMap) Debug.LogError("[RCH] doorMap not assigned.");
        if (!charModel) Debug.LogError("[RCH] charModel not assigned.");
        if (!Grid) Debug.LogError("[RCH] Grid (parent) not assigned.");
        if (!GCH) Debug.LogError("[RCH] GCH not assigned.");

        // cache chair cells
        if (chairTilemap)
        {
            foreach (var cell in chairTilemap.cellBounds.allPositionsWithin)
            {
                if (!chairTilemap.HasTile(cell)) continue;
                chairCells.Add(cell);
                if (!chairOccupied.ContainsKey(cell)) chairOccupied[cell] = false;
            }
        }

        // first door tile
        if (doorMap)
        {
            bool found = false;
            foreach (var cell in doorMap.cellBounds.allPositionsWithin)
            {
                if (!doorMap.HasTile(cell)) continue;
                doorWorld = doorMap.CellToWorld(cell) + doorMap.tileAnchor;
                found = true;
                break;
            }
            if (!found)
            {
                doorWorld = Vector3.zero;
                Debug.LogWarning("[RCH] No door tile found; defaulting to Vector3.zero.");
            }
        }
    }

    void Update()
    {
        if (GCH == null || GCH.customers == null || GCH.customers.Count == 0) return;

        if (enableRandomFlow) AdjustCustomers();

        ProcessLeavers();   // walk out & despawn
        SpawnCustomers();   // spawn & seat
    }

    // ---------- Flow ----------
    private void AdjustCustomers()
    {
        // show drink bubbles
        foreach (var cust in current)
        {
            GameObject go = avatarOf[cust];
            var bubble = go.transform.Find("DrinkRequest");

            if (cust.readyToLeave())
            {
                toLeave.Add(cust);
                cust.inChair = false;
            }
            else if (cust.hasDrink)
            {
                bool finished = cust.finishDrink(TH.getTimePassed());
                if (finished)
                {
                    bubble.gameObject.SetActive(false);
                }
            }
            else if (cust.checkLastDrink(TH.getTimePassed()) && cust.reqDrink == null && cust.inChair)
            {
                string choice = DM.decideDrinkType(cust.beerScore);
                string drinkID;
                if (choice == "beer")
                {
                    drinkID = DM.GetRandomBeerId();
                    Beer beer = DM.FindBeerById(drinkID);
                    bubble.GetComponent<DrinkHandler>().beer = beer;
                    bubble.GetComponent<DrinkHandler>().drink = null;
                }
                else
                {
                    drinkID = DM.decideMixedDrink();
                    MixedDrink md = DM.mixedDrinks[drinkID];
                    bubble.GetComponent<DrinkHandler>().drink = md;
                    bubble.GetComponent<DrinkHandler>().beer = null;
                }
                bubble.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Drinks/exclaim");
                bubble.GetComponent<SellDrink>().revealed = false;
                cust.reqDrink = drinkID;
                cust.startTime = TH.getTimePassed();
                if (bubble) bubble.gameObject.SetActive(true);
            }
            else
            {
                // no-op
            }
        }

        // simple random arrivals
        if (TH.newEvent(lastTime, 5))
        {
            lastTime = TH.getTimePassed();
            int charID = Random.Range(0, GCH.customers.Count);
            int chance = Random.Range(0, 10);

            if (chance > 5)
            {
                var cust = GCH.customers[charID];

                if (!current.Contains(cust) && !toSpawn.Contains(cust) && !toLeave.Contains(cust) &&
                    (toSpawn.Count + current.Count + toLeave.Count) < 10 && !cust.hasVisited)
                {
                    cust.hasVisited = true;
                    toSpawn.Add(cust);
                }
            }
        }
    }

    private void SpawnCustomers()
    {
        if (toSpawn.Count < 1) return;

        var cust = toSpawn[0];
        toSpawn.RemoveAt(0);
        current.Add(cust);
        cust.lastDrink = Time.time;

        if (!charModel || !Grid) { Debug.LogWarning("[RCH] Missing charModel or Grid."); return; }

        // spawn at door
        var avatar = CreateCharacterObject(cust, doorWorld);

        // pick a free seat
        var seatCell = AssignChairCell();
        if (IsValidCell(seatCell))
        {
            seatCellOf[cust] = seatCell;
            var seatWorld = WorldFromCell(seatCellOf[cust]);
            StartMove(cust, avatar, seatWorld, walkSpeed);
        }
        else
        {
            Debug.LogWarning("[RCH] No available chairs; customer waits at door.");
        }

        avatarOf[cust] = avatar;
    }

    private void ProcessLeavers()
    {
        if (toLeave.Count < 1) return;

        var cust = toLeave[0];
        toLeave.RemoveAt(0);
        current.Remove(cust);

        // free seat
        if (seatCellOf.TryGetValue(cust, out var seatCell))
        {
            FreeChair(seatCell);
            seatCellOf.Remove(cust);
        }

        if (avatarOf.TryGetValue(cust, out var avatar) && avatar)
        {
            StartMove(cust, avatar, doorWorld, walkSpeed, () => SafeDespawn(cust, avatar));
        }
    }

    // ---------- Seats ----------
    private Vector3 WorldFromCell(Vector3Int cell) =>
        chairTilemap.CellToWorld(cell) + chairTilemap.tileAnchor;

    private bool IsValidCell(Vector3Int c) =>
        c.x != int.MinValue || c.y != int.MinValue || c.z != int.MinValue;

    private Vector3Int AssignChairCell()
    {
        foreach (var cell in chairCells)
        {
            if (!chairOccupied.TryGetValue(cell, out var occ) || !occ)
            {
                chairOccupied[cell] = true;
                return cell;
            }
        }
        return new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    }

    private void FreeChair(Vector3Int cell)
    {
        if (chairOccupied.ContainsKey(cell)) chairOccupied[cell] = false;
    }

    // ---------- Avatars ----------
    private GameObject CreateCharacterObject(Customer cust, Vector3 worldPos)
    {
        var go = Instantiate(charModel);
        go.transform.SetParent(Grid.transform, worldPositionStays: true);
        go.transform.position = worldPos; // world-space

        // optional label/sprite wiring
        var label = go.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null) label.text = cust.name;

        var handler = go.GetComponentInChildren<CustomerHandler>(true);
        if (handler != null) handler.customer = cust;

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null && handler != null && handler.sprite != null)
        {
            sr.sprite = handler.sprite;
            sr.sortingOrder = 5;
        }

        var bubble = go.transform.Find("DrinkRequest");
        if (bubble != null) bubble.gameObject.SetActive(false);

        return go;
    }

    // ---------- Movement (world-space only) ----------
    private void StartMove(Customer cust, GameObject obj, Vector3 targetWorld, float speed, System.Action onComplete = null)
    {
        if (moveRoutineOf.TryGetValue(cust, out var existing) && existing != null)
            StopCoroutine(existing);

        IEnumerator routine;

        if (gridlikeMovement && RefGrid)
        {
            // Grid stepping: one axis per stepInterval
            routine = MoveToSpotGrid(obj, targetWorld, () =>
            {
                moveRoutineOf.Remove(cust);
                cust.inChair = true;
                onComplete?.Invoke();
            });
        }
        else
        {
            // Original smooth mover
            routine = MoveToSpot(obj, targetWorld, speed, () =>
            {
                moveRoutineOf.Remove(cust);
                onComplete?.Invoke();
                cust.inChair = true;
            });
        }

        var co = StartCoroutine(routine);
        moveRoutineOf[cust] = co;
    }

    // Original smooth mover (unchanged)
    private IEnumerator MoveToSpot(GameObject character, Vector3 targetWorld, float speed, System.Action onComplete = null)
    {
        if (!character) yield break;

        float elapsed = 0f;
        try
        {
            while (character && elapsed < hardMoveTimeout)
            {
                var pos = character.transform.position;

                if (Vector3.Distance(pos, targetWorld) <= arriveThreshold) break;

                character.transform.position =
                    Vector3.MoveTowards(pos, targetWorld, speed * Time.deltaTime);

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (character) character.transform.position = targetWorld;
        }
        finally
        {
            onComplete?.Invoke();
        }
    }

    // NEW: Grid-stepping mover (one axis per second, avoids obstacleTilemap)
    private IEnumerator MoveToSpotGrid(GameObject character, Vector3 targetWorld, System.Action onComplete = null)
    {
        if (!character || !RefGrid) yield break;

        float elapsedTotal = 0f;

        Vector3Int currentCell = RefGrid.WorldToCell(character.transform.position);
        Vector3Int targetCell  = RefGrid.WorldToCell(targetWorld);

        // Snap to center of starting cell if needed
        Vector3 currentCenter = CenterFromCell(RefGrid, currentCell);
        if ((character.transform.position - currentCenter).sqrMagnitude > 0.0001f)
        {
            float t = 0f;
            Vector3 startPos = character.transform.position;
            while (t < 1f)
            {
                character.transform.position = Vector3.Lerp(startPos, currentCenter, t);
                t += Time.deltaTime / Mathf.Max(0.0001f, stepInterval);
                elapsedTotal += Time.deltaTime;
                if (elapsedTotal >= hardMoveTimeout) goto EXIT;
                yield return null;
            }
            character.transform.position = currentCenter;
        }

        while (currentCell != targetCell && elapsedTotal < hardMoveTimeout)
        {
            int dx = targetCell.x - currentCell.x;
            int dy = targetCell.y - currentCell.y;

            Vector3Int nextCell = currentCell;

            // primary axis (greater magnitude)
            bool chose = false;
            if (Mathf.Abs(dx) >= Mathf.Abs(dy) && dx != 0)
            {
                nextCell = new Vector3Int(currentCell.x + Mathf.Clamp(dx, -1, 1), currentCell.y, currentCell.z);
                chose = true;
            }
            else if (dy != 0)
            {
                nextCell = new Vector3Int(currentCell.x, currentCell.y + Mathf.Clamp(dy, -1, 1), currentCell.z);
                chose = true;
            }
            if (!chose) break;

            // if blocked, try alternate axis
            if (IsBlockedCell(nextCell))
            {
                if (Mathf.Abs(dx) < Mathf.Abs(dy) && dx != 0)
                    nextCell = new Vector3Int(currentCell.x + Mathf.Clamp(dx, -1, 1), currentCell.y, currentCell.z);
                else if (dy != 0)
                    nextCell = new Vector3Int(currentCell.x, currentCell.y + Mathf.Clamp(dy, -1, 1), currentCell.z);

                // still blocked? wait one interval and retry
                if (IsBlockedCell(nextCell))
                {
                    float wait = 0f;
                    while (wait < stepInterval)
                    {
                        wait += Time.deltaTime;
                        elapsedTotal += Time.deltaTime;
                        if (elapsedTotal >= hardMoveTimeout) goto EXIT;
                        yield return null;
                    }
                    continue;
                }
            }

            // animate to next cell over exactly one stepInterval
            Vector3 start = character.transform.position;
            Vector3 end = CenterFromCell(RefGrid, nextCell);

            float tStep = 0f;
            while (tStep < 1f)
            {
                character.transform.position = Vector3.Lerp(start, end, tStep);
                tStep += Time.deltaTime / Mathf.Max(0.0001f, stepInterval);
                elapsedTotal += Time.deltaTime;
                if (elapsedTotal >= hardMoveTimeout) goto EXIT;
                yield return null;
            }
            character.transform.position = end;

            currentCell = nextCell;
        }

        // Close enough to exact target? snap to anchor (covers seat anchor offsets)
        if ((character.transform.position - targetWorld).sqrMagnitude <= arriveThreshold * arriveThreshold)
            character.transform.position = targetWorld;

    EXIT:
        onComplete?.Invoke();
    }

    // ---------- Despawn ----------
    private void SafeDespawn(Customer cust, GameObject obj)
    {
        if (moveRoutineOf.TryGetValue(cust, out var co) && co != null)
        {
            StopCoroutine(co);
            moveRoutineOf.Remove(cust);
        }

        if (obj) Destroy(obj);
        avatarOf.Remove(cust);
    }
}
