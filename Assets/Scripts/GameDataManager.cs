// Add System.IO to work with files!
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GameDataManager : MonoBehaviour
{
    // Create a field for the save file.
    string barStatusFile;
    string beerLibraryFile;
    string ingredientLibraryFile;
    string recipeLibraryFile;


    void Awake()
    {
        // Update the path once the persistent path exists.
        barStatusFile = Application.persistentDataPath + "/barStatusFile.json";
        beerLibraryFile = Application.persistentDataPath + "/beerLibraryFile.json";
        ingredientLibraryFile = Application.persistentDataPath + "/ingredientLibraryFile.json";
        recipeLibraryFile = Application.persistentDataPath + "/recipeLibraryFile.json";
    }
    
    public void updateBarStatus(BarHandler bh)
    {
        string jsonString = JsonConvert.SerializeObject(bh);
        
        File.WriteAllText(barStatusFile, jsonString);
    }

    public BarHandler loadBarStatus()
    {
        if (File.Exists(barStatusFile))
        {
            string FileContains = File.ReadAllText(barStatusFile);
            return JsonConvert.DeserializeObject<BarHandler>(FileContains);
        }
        else
        {
            BarHandler bh = new BarHandler();
            updateBarStatus(bh);
            return bh;
        }
    }

    public Dictionary<string, List<Beer>> loadBeers()
    {
        Dictionary<string, List<Beer>> beers = new Dictionary<string, List<Beer>>();
        // Does the file exist?
        if (File.Exists(beerLibraryFile))
        {
            // Read the entire file and save its contents.
            string FileContains = File.ReadAllText(beerLibraryFile);
            string[] fc = FileContains.Split("\n", System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string data in fc)
            {
                Beer b = JsonUtility.FromJson<Beer>(data);
                string keyToCheck = b.type;
                if (beers.ContainsKey(keyToCheck))
                {
                    beers[keyToCheck].Add(b);
                }
                else
                {
                    beers[keyToCheck] = new List<Beer>();
                    beers[keyToCheck].Add(b);
                }
            }

        }
        
        return beers;
     
    }

    public void SaveBeers(Dictionary<string, List<Beer>> beers)
    {
        if (beers == null) return;

        // Ensure folder exists
        var dir = Path.GetDirectoryName(beerLibraryFile);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        // Flatten to newline-delimited JSON, stable order (by key then name) is nice but optional
        var lines = new List<string>();
        foreach (var kvp in beers)
        {
            var list = kvp.Value;
            if (list == null) continue;

            foreach (var d in list)
            {
                if (d == null) continue;
                lines.Add(JsonUtility.ToJson(d));
            }
        }

        // Write atomically
        var tmp = beerLibraryFile + ".tmp";
        var payload = string.Join("\n", lines) + "\n"; // final newline so Split(..., RemoveEmptyEntries) works cleanly
        File.WriteAllText(tmp, payload);

        if (File.Exists(beerLibraryFile))
        {
            // Replace keeps file attributes on supported platforms
            File.Replace(tmp, beerLibraryFile, null);
        }
        else
        {
            File.Move(tmp, beerLibraryFile);
        }
    }

    public Dictionary<string, List<Ingredient>> loadIngredients()
    {
        Dictionary<string, List<Ingredient>> ingredients = new Dictionary<string, List<Ingredient>>();
        // Does the file exist?
        if (File.Exists(ingredientLibraryFile))
        {
            // Read the entire file and save its contents.
            string FileContains = File.ReadAllText(ingredientLibraryFile);
            string[] fc = FileContains.Split("\n", System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string data in fc)
            {
                Ingredient i = JsonUtility.FromJson<Ingredient>(data);
                string keyToCheck = i.type;
                if (ingredients.ContainsKey(keyToCheck))
                {
                    ingredients[keyToCheck].Add(i);
                }
                else
                {
                    ingredients[keyToCheck] = new List<Ingredient>();
                    ingredients[keyToCheck].Add(i);
                }
            }

        }
        
        return ingredients;
     
    }

    public void SaveIngredients(Dictionary<string, List<Ingredient>> ingredients)
    {
        if (ingredients == null) return;

        // Ensure folder exists
        var dir = Path.GetDirectoryName(ingredientLibraryFile);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        // Flatten to newline-delimited JSON, stable order (by key then name) is nice but optional
        var lines = new List<string>();
        foreach (var kvp in ingredients)
        {
            var list = kvp.Value;
            if (list == null) continue;

            foreach (var d in list)
            {
                if (d == null) continue;
                lines.Add(JsonUtility.ToJson(d));
            }
        }

        // Write atomically
        var tmp = ingredientLibraryFile + ".tmp";
        var payload = string.Join("\n", lines) + "\n"; // final newline so Split(..., RemoveEmptyEntries) works cleanly
        File.WriteAllText(tmp, payload);

        if (File.Exists(ingredientLibraryFile))
        {
            // Replace keeps file attributes on supported platforms
            File.Replace(tmp, ingredientLibraryFile, null);
        }
        else
        {
            File.Move(tmp, ingredientLibraryFile);
        }
    }

    
    public Dictionary<string, MixedDrink> loadMixedDrinks()
    {
        Dictionary<string, MixedDrink> mixedDrinks = new Dictionary<string, MixedDrink>();
        // Does the file exist?
        if (File.Exists(recipeLibraryFile))
        {
            // Read the entire file and save its contents.
            string FileContains = File.ReadAllText(recipeLibraryFile);
            string[] fc = FileContains.Split("\n", System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string data in fc)
            {
                MixedDrink m = JsonUtility.FromJson<MixedDrink>(data);
                string keyToCheck = m.id;
                
                mixedDrinks[keyToCheck] = m;
                
            }

        }
        
        return mixedDrinks;
     
    }

}