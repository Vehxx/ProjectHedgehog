using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLiquorOptions : MonoBehaviour
{
    public DrinkManager dm;
    public GameObject panel;
    public GameObject panel2;
    public GameObject button;
    public Dictionary<string, List<Ingredient>> ingredients;
    public List<string> sortedIngredients;
    public List<GameObject> curButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        ingredients = dm.ingredients;
        sortedIngredients = new List<string>(ingredients.Keys);
        sortedIngredients.Remove("Garnish");
        int minval = sortedIngredients.Count;
        if (minval != curButtons.Count) {
            foreach(GameObject but in curButtons){
                Destroy(but);
            }
            CreateButtons(sortedIngredients);
        } 
    }

    private void CreateButtons(List<string> ings)
    {
        int minval = ings.Count;
        for (int i = 0; i < minval; i++) 
        {
            string ing = ings[i];
            Vector3 position = new Vector3(0,0,0);
            CreateButton(ing,position);
        }
    }

    
    private void CreateButton(string ing, Vector3 position)
    {
        // Use the overload that sets the parent to preserve local layout
        GameObject newButton = Instantiate(button, panel.transform);
        newButton.transform.localPosition = position; // or use a LayoutGroup and skip manual positions

        // Set UI
        newButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ing;

        var loader = newButton.GetComponentInChildren<SetBottlePanel>(true); 
        loader.panel = panel2;
        loader.txt = ing;
        //loader.panel = panel2;
        //loader.bom = bom;

        curButtons.Add(newButton);
    }
}
