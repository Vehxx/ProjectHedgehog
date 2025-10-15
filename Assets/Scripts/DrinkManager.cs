using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkManager : MonoBehaviour
{

    public Dictionary<string, List<Beer>> beers {get;set;}
    public Dictionary<string, List<Ingredient>> ingredients {get;set;}
    public Dictionary<string, MixedDrink> mixedDrinks {get;set;}
    public GameDataManager gdm;

    // Start is called before the first frame update
    void Start()
    {
        beers = gdm.loadBeers();
        ingredients = gdm.loadIngredients();
        mixedDrinks = gdm.loadMixedDrinks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string decideDrinkType(float beerPercent)
    {

        int randomValue = Random.Range(0, 100); // Generates a number between 0 and 99

        // Define percentages
        float option1Percentage = beerPercent*100;

        // Determine the outcome
        if (randomValue < option1Percentage)
        {
            return "beer";
        }
        else
        {
            return "mixedDrink";
        }
    }

    public string decideMixedDrink() {

        List<string> keys = new List<string>(mixedDrinks.Keys);
        int drinkChoice = Random.Range(0, keys.Count);
        
        return keys[drinkChoice];

    }


    public string GetRandomBeerId()
    {
        // total number of beers across all types
        int total = 0;
        foreach (var list in beers.Values)
            if (list != null) total += list.Count;

        int idx = Random.Range(0, total);
        // walk lists until we land on the idx-th beer
        foreach (var list in beers.Values)
        {
            if (list == null) continue;
            if (idx < list.Count)
                return list[idx].id;

            idx -= list.Count;
        }

        return null;


    }

    /// Finds a beer by ID; returns null if not found.
    public Beer FindBeerById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        foreach (var list in beers.Values)
        {
            if (list == null) continue;
            for (int i = 0; i < list.Count; i++)
            {
                var b = list[i];
                if (string.Equals(b.id, id))
                    return b;
            }
        }
        return null;
    }
}
