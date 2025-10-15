using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeerTapPanel : MonoBehaviour
{
    public DrinkManager dm;
    public GameHandler gh;
    public GameObject panel;
    public GameObject button;
    public GameObject player;
    //public static StringComparer Ordinal { get; }
    public Dictionary<string, List<Beer>> beers;
    public List<Beer> sortedBeers;
    public string type = "Ipa";
    //int crew_amount = 0;
    public List<GameObject> curButtons = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        beers = dm.beers;
        sortedBeers = beers[type];
    }

    // Update is called once per frame
    void Update()
    {
        int minval = Mathf.Min(3,sortedBeers.Count);
        if (minval != curButtons.Count) {
            foreach(GameObject but in curButtons){
                Destroy(but);
            }
            CreateButtons(sortedBeers);
        } 
    }

    private void CreateButtons(List<Beer> b)
    {
        int minval = Mathf.Min(b.Count,3);
        for (int i = 0; i < minval; i++) 
        {
            Beer br = b[i];
            Vector3 position = new Vector3(0,0,0);
            CreateButton(br,position);
        }
    }

    private void CreateButton(Beer b, Vector3 position)
    {
        // Use the overload that sets the parent to preserve local layout
        GameObject newButton = Instantiate(button, panel.transform);
        newButton.transform.localPosition = position; // or use a LayoutGroup and skip manual positions

        // Set UI
        newButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = b.name;
        newButton.transform.GetChild(1).GetComponent<Image>().sprite =
            Resources.Load<Sprite>($"Drinks/{b.logo_sprite}");

        // Pass instance data to the handler that the prefab’s OnClick will use
        var handler = newButton.GetComponentInChildren<DrinkHandler>(true);
        handler.beer = b;

        var draftBeer = newButton.GetComponentInChildren<DraftBeer>(true);
        draftBeer.player = player;
        draftBeer.panel = panel;
        draftBeer.button = newButton;

        curButtons.Add(newButton);
    }
}
