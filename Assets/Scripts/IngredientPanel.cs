using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientPanel : MonoBehaviour
{
    public DrinkManager dm;
    public GameHandler gh;
    public GameObject panel;
    public GameObject button;
    public GameObject player;
    public Dictionary<string, List<Ingredient>> ingredients = new Dictionary<string, List<Ingredient>>();
    public List<Ingredient> sortedIngredients;
    public BarTenderHandler BTH;
    public String type = "None";
    public String oldType = "None";
    public List<GameObject> curButtons = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ingredients = dm.ingredients;
        if (!ingredients.ContainsKey(type)) {
            return;
        } else {
            sortedIngredients = ingredients[type];
        }
        int minval = sortedIngredients.Count;
        if (oldType != type) {
            foreach(GameObject but in curButtons) {
                Destroy(but);
            }
            oldType = type;
            CreateButtons(sortedIngredients);
        }
        
    }

    private void CreateButtons(List<Ingredient> ingr)
    {
        for (int i = 0; i < ingr.Count; i++) 
        {
            Ingredient ing = ingr[i];
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

        // Pass instance data to the handler that the prefab’s OnClick will use
        var handler = newButton.GetComponentInChildren<IngredientHandler>(true);
        handler.ingredient = ing;

        var addIngr = newButton.GetComponentInChildren<AddIngredient>(true);
        //draftBeer.gh = gh;
        addIngr.button = newButton;
        addIngr.player = BTH;
        //panel = panel;

        curButtons.Add(newButton);
    }
}
