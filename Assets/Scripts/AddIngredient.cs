using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddIngredient : MonoBehaviour
{
    public BarTenderHandler player;
    public GameObject button;

    public void clicked()
    {
        Ingredient ingredient = button.GetComponentInChildren<IngredientHandler>(true).ingredient;

        if (ingredient.amount < 1)
        {
            return;
        }
  
        

        if (player.inventory.Count < 5)
        {
            ingredient.amount --;
            player.inventory.Add(ingredient);
        }
    }
}
