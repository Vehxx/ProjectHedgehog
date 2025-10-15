using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadIngrItem : MonoBehaviour
{
    public DrinkManager dm;
    public GameObject panel;
    public GameObject button;
    public BeforeOpenManager bom;
    //public static StringComparer Ordinal { get; }
    public Dictionary<string, List<Ingredient>> ingredients;
    public List<Ingredient> sortedIngredients;
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
        ingredients = dm.ingredients;
        sortedIngredients = ingredients[type];

        int minval = Mathf.Min(3,sortedIngredients.Count);
        if (minval != curButtons.Count) {
            foreach(GameObject but in curButtons){
                Destroy(but);
            }
            CreateButtons(sortedIngredients);
        } 
    }

    private void CreateButtons(List<Ingredient> ings)
    {
        int minval = Mathf.Min(ings.Count,3);
        for (int i = 0; i < minval; i++) 
        {
            Ingredient ing = ings[i];
            Vector3 position = new Vector3(0,0,0);
            CreateButton(ing,position);
        }
    }

    private void CreateButton(Ingredient ing, Vector3 position)
    {
        // Use the overload that sets the parent to preserve local layout
        GameObject newButton = Instantiate(button, panel.transform);
        newButton.transform.localPosition = position; // or use a LayoutGroup and skip manual positions

        // Set UI
        newButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ing.name;
        newButton.transform.GetChild(1).GetComponent<Image>().sprite =
            Resources.Load<Sprite>($"Ingredients/{ing.sprite}");

        var buyer = newButton.GetComponentInChildren<BuyIngredient>(true); 

        buyer.bom = bom;

        newButton.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>().text = "Buy Price: " + ing.cost.ToString();
        newButton.transform.GetChild(7).GetComponent<TMPro.TextMeshProUGUI>().text = "Inventory Amount: " + ing.amount.ToString();

        // Pass instance data to the handler that the prefab’s OnClick will use
        var handler = newButton.GetComponentInChildren<IngredientHandler>(true);
        handler.ingredient = ing;

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
