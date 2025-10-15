using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class BuyIngredient : MonoBehaviour
{
    public BeforeOpenManager bom;
    public GameObject panel;
    public TMP_Text buyAmount;
    public TMP_Text newAmount;


    public void clicked()
    {
        Transform quad = panel.transform.Find("ItemHandler");
        GameObject go2 = quad.gameObject;
        var ingredient = go2.GetComponent<IngredientHandler>().ingredient;

        string textValue = buyAmount.text;

        string cleaned = Regex.Replace(textValue, @"[^\+\-0-9]", ""); // keep + - digits only

        int number = int.Parse(cleaned);

        if (ingredient != null) {
            if (bom.bh.MoneyCount > ((float) number) * ingredient.cost)
            {
                ingredient.amount += (int) number;
                bom.bh.BuyBeer(((float) number) * ingredient.cost);
                newAmount.text = "Inventory Amount: " + ingredient.amount.ToString();
            }
        }

        
    }
}
