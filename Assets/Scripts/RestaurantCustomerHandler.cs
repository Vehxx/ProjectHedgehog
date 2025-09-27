using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class RestaurantCustomerHandler : MonoBehaviour
{
    public GlobalCustomerHandler GCH;
    [SerializeField] private GameObject charModel;
    [SerializeField] private GameObject Grid;                // parent container (world object OR UI)
    [SerializeField] private Tilemap chairTilemap;
    [SerializeField] private Tilemap doorMap;

    [SerializeField] private bool enableRandomFlow = true;

    public List<Customer> toSpawn = new();
    public List<Customer> current = new();
    public List<Customer> toLeave = new();

    // internals
    private readonly List<Vector3> chairPositions = new();
    private readonly Dictionary<Vector3, bool> chairOccupied = new();       // chair pos -> occupied?
    private readonly Dictionary<Customer, Vector3> seatOf = new();          // customer -> chair pos
    private readonly Dictionary<Customer, GameObject> avatarOf = new();     // customer -> avatar
    private readonly Dictionary<Customer, Coroutine> moveRoutineOf = new(); // customer -> active move coroutine

    private Vector3 doorlocation;
    private const float walkSpeed = 2f;

    // cached UI context (if Grid is under a Canvas)
    private Canvas parentCanvas;
    private RectTransform gridRect;
    private bool gridHasLayout;

    void Awake()
    {
        if (Grid != null)
        {
            gridRect = Grid.GetComponent<RectTransform>();
            parentCanvas = Grid.GetComponentInParent<Canvas>();
            gridHasLayout = Grid.GetComponent<LayoutGroup>() != null || Grid.GetComponent<GridLayoutGroup>() != null;
        }
    }

    void Start()
    {
        // sanity checks
        if (chairTilemap == null) Debug.LogError("[RCH] chairTilemap not assigned.");
        if (doorMap == null) Debug.LogError("[RCH] doorMap not assigned.");
        if (charModel == null) Debug.LogError("[RCH] charModel not assigned.");
        if (Grid == null) Debug.LogError("[RCH] Grid not assigned.");
        if (GCH == null) Debug.LogError("[RCH] GCH not assigned.");

        // cache chair positions
        if (chairTilemap != null)
        {
            foreach (Vector3Int cell in chairTilemap.cellBounds.allPositionsWithin)
            {
                if (!chairTilemap.HasTile(cell)) continue;
                Vector3 world = chairTilemap.CellToWorld(cell) + chairTilemap.tileAnchor;
                if (!chairOccupied.ContainsKey(world))
                {
                    chairPositions.Add(world);
                    chairOccupied[world] = false;
                }
            }
        }

        // door position (first door tile)
        if (doorMap != null)
        {
            bool found = false;
            foreach (Vector3Int cell in doorMap.cellBounds.allPositionsWithin)
            {
                if (!doorMap.HasTile(cell)) continue;
                doorlocation = doorMap.CellToWorld(cell) + doorMap.tileAnchor; // correct map
                found = true;
                break;
            }
            if (!found)
            {
                Debug.LogWarning("[RCH] No door tile found; defaulting to Vector3.zero.");
                doorlocation = Vector3.zero;
            }
        }


        // warn if UI layout will override positioning
        if (gridRect != null && gridHasLayout)
        {
            Debug.LogWarning("[RCH] Grid has a LayoutGroup/GridLayoutGroup. It will override child positions. " +
                             "Disable it if you want manual placement/movement over the tilemap.");
        }
    }

    void Update()
    {
        if (GCH == null || GCH.customers == null || GCH.customers.Count == 0) return;

        if (enableRandomFlow)
        {
            AdjustCustomers();  // randomly queue spawns/leaves
        }

        ProcessLeavers();   // walk out, free chairs, despawn
        SpawnCustomers();   // spawn and seat newcomers
    }

    // randomly queue customers to spawn/leave
    private void AdjustCustomers()
    {
        int charID = Random.Range(0, GCH.customers.Count); // upper bound exclusive
        int chance = Random.Range(0, 1000);                  // 0..49

        foreach (Customer cust in current) {
            bool wantsDrink = cust.checkLastDrink(Time.time);
            if (wantsDrink) {
                GameObject go1 = avatarOf[cust];
                Transform quad = go1.transform.Find("Quad");
                GameObject go2 = quad.gameObject;
                go2.SetActive(true);
            }
        }

        if (chance > 950)
        {
            Customer cust = GCH.customers[charID];

            if (!current.Contains(cust) && !toSpawn.Contains(cust) && !toLeave.Contains(cust) && (toSpawn.Count + current.Count + toLeave.Count) < 6) {
                cust.setStartTime(Time.time);
                toSpawn.Add(cust);
            } else if (current.Contains(cust) && !toLeave.Contains(cust) && ((Time.time - cust.getStartTime()) > 10)) {
                Debug.Log(Time.time - cust.getStartTime());
                //toLeave.Add(cust);
            } else {
                
            }
        }
    }

    private void SpawnCustomers()
    {
        if (toSpawn.Count >= 1)
        {
            Customer cust = toSpawn[0];
            toSpawn.RemoveAt(0);
            current.Add(cust);
            cust.setDrinkTime(Time.time);

            if (charModel == null || Grid == null)
            {
                Debug.LogWarning("[RCH] Cannot spawn: missing charModel or Grid.");
            }

            Vector3 spawnWorld = doorlocation;
            GameObject obj = CreateCharacterObject(cust, spawnWorld);

            Vector3 targetSpot = AssignChair();
            if (targetSpot != Vector3.zero)
            {
                seatOf[cust] = targetSpot;
                StartMove(cust, obj, targetSpot, walkSpeed);
            }
            else
            {
                Debug.LogWarning("[RCH] No available chairs; customer waits at door.");
            }

            //current.Add(cust);
            avatarOf[cust] = obj;

            //Debug.Log($"[RCH] Spawned {cust.getFirstName()} at {spawnWorld} (UI={(gridRect!=null)})");
        } 
    }

    private void ProcessLeavers()
    {
        if (toLeave.Count >= 1)
        {
            Customer cust = toLeave[0];
            current.Remove(cust);
            toLeave.RemoveAt(0);

            // free chair immediately
            if (seatOf.TryGetValue(cust, out Vector3 seat))
            {
                if (chairOccupied.ContainsKey(seat)) chairOccupied[seat] = false;
                seatOf.Remove(cust);
            }

            if (avatarOf.TryGetValue(cust, out GameObject obj) && obj != null)
            {
                StartMove(cust, obj, doorlocation, walkSpeed, () => SafeDespawn(cust, obj));
                avatarOf.Remove(cust);
            }
            
        }
    }

    // --- UI/world safe instantiation & positioning ---
    private GameObject CreateCharacterObject(Customer cust, Vector3 worldPosition)
    {
        GameObject go = Instantiate(charModel);
        go.transform.SetParent(Grid.transform, false);

        // Set text/handler if present
        var label = go.transform.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null) label.text = cust.getFirstName();

        var handler = go.GetComponentInChildren<CustomerHandler>(true);
        if (handler != null) handler.customer = cust;

        go.GetComponent<SpriteRenderer>().sprite = handler.sprite;
        go.GetComponent<SpriteRenderer>().sortingOrder  = 5;

        Transform quad = go.transform.Find("Quad");
        GameObject go2 = quad.gameObject;
        go2.SetActive(false);

        // Place depending on parent type
        if (gridRect != null && parentCanvas != null)
        {
            // Parent is UI: convert world -> canvas local
            Vector2 screen = Camera.main != null
                ? (Vector2)Camera.main.WorldToScreenPoint(worldPosition)
                : new Vector2(worldPosition.x, worldPosition.y);

            Vector2 local;
            var cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, screen, cam, out local))
            {
                var childRect = go.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    childRect.anchoredPosition = local;
                }
                else
                {
                    // fallback
                    go.transform.localPosition = local;
                }
            }
        }
        else
        {
            // Parent is a world object (no Canvas): set world position
            go.transform.position = worldPosition;
        }

        // optional: keep hierarchy tidy
        int maxIndex = Mathf.Max(0, go.transform.parent.childCount - 1);
        go.transform.SetSiblingIndex(Mathf.Min(6, maxIndex));

        return go;
    }

    // Move respecting UI/world parent
    private void StartMove(Customer cust, GameObject obj, Vector3 targetWorld, float speed, System.Action onComplete = null)
    {
        if (moveRoutineOf.TryGetValue(cust, out var existing) && existing != null)
        {
            StopCoroutine(existing);
        }

        var co = StartCoroutine(MoveToSpot(obj, targetWorld, speed, () =>
        {
            moveRoutineOf.Remove(cust);
            onComplete?.Invoke();
        }));

        moveRoutineOf[cust] = co;
    }

    private IEnumerator MoveToSpot(GameObject character, Vector3 targetWorld, float speed, System.Action onComplete = null)
{
    if (character == null) yield break;

    RectTransform rect = (gridRect != null && parentCanvas != null) ? character.GetComponent<RectTransform>() : null;

    const float hardTimeout = 10f;
    float elapsed = 0f;

    if (rect != null)
    {
        // Compute target once in canvas-local (anchored) space
        Vector2 screen = Camera.main != null
            ? (Vector2)Camera.main.WorldToScreenPoint(targetWorld)
            : new Vector2(targetWorld.x, targetWorld.y);

        Vector2 targetLocal;
        var cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, screen, cam, out targetLocal))
            yield break;

        while (character != null && elapsed < hardTimeout)
        {
            Vector2 current = rect.anchoredPosition;
            if (Vector2.Distance(current, targetLocal) <= 0.05f) break;

            Vector2 next = Vector2.MoveTowards(current, targetLocal, speed * Time.deltaTime * 100f);
            rect.anchoredPosition = next;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (character != null) rect.anchoredPosition = targetLocal;
    }
    else
    {
        while (character != null && elapsed < hardTimeout)
        {
            if (Vector3.Distance(character.transform.position, targetWorld) <= 0.05f) break;

            character.transform.position = Vector3.MoveTowards(
                character.transform.position, targetWorld, speed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (character != null) character.transform.position = targetWorld;
    }

    onComplete?.Invoke();
}

    // pick a free chair
    private Vector3 AssignChair()
    {
        foreach (Vector3 pos in chairPositions)
        {
            if (!chairOccupied[pos])
            {
                chairOccupied[pos] = true;
                return pos;
            }
        }
        return Vector3.zero;
    }

    // stop any active move and destroy safely
    private void SafeDespawn(Customer cust, GameObject obj)
{
    if (moveRoutineOf.TryGetValue(cust, out var co) && co != null)
    {
        StopCoroutine(co);
        moveRoutineOf.Remove(cust);
    }

    if (obj != null)
    {
        Destroy(obj);
    }
}
}
