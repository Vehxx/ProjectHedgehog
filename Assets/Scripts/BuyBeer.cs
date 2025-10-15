using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class BuyBeer : MonoBehaviour
{
    public BeforeOpenManager bom;
    public GameObject panel;
    public TMP_Text buyAmount;
    public TMP_Text newAmount;


    public void clicked()
    {
        Transform quad = panel.transform.Find("ItemHandler");
        GameObject go2 = quad.gameObject;
        var beer = go2.GetComponent<DrinkHandler>().beer;

        string textValue = buyAmount.text;

        string cleaned = Regex.Replace(textValue, @"[^\+\-0-9]", ""); // keep + - digits only

        int number = int.Parse(cleaned);

        if (beer != null) {
            if (bom.bh.MoneyCount > ((float) number) * beer.buy_price)
            {
                beer.amount += (int) number;
                bom.bh.BuyBeer(((float) number) * beer.buy_price);
                newAmount.text = "Inventory Amount: " + beer.amount.ToString();
            }
        }
        
    }
}
