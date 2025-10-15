using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MixedDrink
{
    public string id;
    public string name;
    public string sprite;
    public float sell_price;
    public float alcohol;
    public string type;
    public List<Ingredient> ingredients;
}
