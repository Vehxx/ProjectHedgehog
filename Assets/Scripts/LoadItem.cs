using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadItem : MonoBehaviour
{
    public DrinkManager dm;
    public GameObject panel;
    public GameObject button;
    public BeforeOpenManager bom;
    //public static StringComparer Ordinal { get; }
    public Dictionary<string, List<Beer>> beers;
    public List<Beer> sortedBeers;
    public string type = "";
    //int crew_amount = 0;
    public List<GameObject> curButtons = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void clicked(GameObject buttonClicked)
    {
        string type = buttonClicked.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;

        //if (type == newType) 
        //{
        //    return;
        //}
        ClearPanel(panel);
        //type = newType;
        beers = dm.beers;
        sortedBeers = beers[type];

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

        var buyer = newButton.GetComponentInChildren<BuyBeer>(true); 

        buyer.bom = bom;

        newButton.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().text = "Buy Price: " + b.buy_price.ToString();
        newButton.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().text = "Sell Price: " + b.sell_price.ToString();
        newButton.transform.GetChild(8).GetComponent<TMPro.TextMeshProUGUI>().text = "Inventory Amount: " + b.amount.ToString();
        newButton.transform.GetChild(9).GetComponent<TMPro.TextMeshProUGUI>().text = "ABV: " + b.alcohol.ToString();

        // Pass instance data to the handler that the prefab’s OnClick will use
        var handler = newButton.GetComponentInChildren<DrinkHandler>(true);
        handler.beer = b;

        curButtons.Add(newButton);
    }

    public void ClearPanel(GameObject panelGO)
    {
        if (panelGO == null) return;

        Transform t = panelGO.transform;

        // Destroy children from last to first
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            Destroy(t.GetChild(i).gameObject);
        }

        // If this panel participates in a layout, refresh it immediately
        var rt = panelGO.GetComponent<RectTransform>();
        if (rt != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

        curButtons = new List<GameObject>();
    }
}
