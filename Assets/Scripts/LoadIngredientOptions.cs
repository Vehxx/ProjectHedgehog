using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LoadIngredientOptions : MonoBehaviour
{
    public DrinkManager dm;
    public GameObject panel;
    public GameObject panel2;
    public BeforeOpenManager bom;
    public GameObject button;
    public Dictionary<string, List<Ingredient>> ingredients;
    public List<string> sortedIngredients;
    public List<GameObject> curButtons = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    public void clicked()
    {
        ClearPanel(panel);
        ingredients = dm.ingredients;
        sortedIngredients = new List<string>();
        sortedIngredients.Add("Garnish");
        int minval = Mathf.Min(5,sortedIngredients.Count);
        if (minval != curButtons.Count) {
            foreach(GameObject but in curButtons){
                Destroy(but);
            }
            CreateButtons(sortedIngredients);
        } 
    }

    private void CreateButtons(List<string> ing)
    {
        int minval = Mathf.Min(ing.Count,5);
        for (int i = 0; i < minval; i++) 
        {
            string ingr = ing[i];
            Vector3 position = new Vector3(0,0,0);
            CreateButton(ingr,position);
        }
    }

    
    private void CreateButton(string ing, Vector3 position)
    {
        // Use the overload that sets the parent to preserve local layout
        GameObject newButton = Instantiate(button, panel.transform);
        newButton.transform.localPosition = position; // or use a LayoutGroup and skip manual positions

        // Set UI
        newButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ing;

        var loader = newButton.GetComponentInChildren<LoadIngrItem>(true); 
        loader.dm = dm;
        loader.panel = panel2;
        loader.bom = bom;

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
