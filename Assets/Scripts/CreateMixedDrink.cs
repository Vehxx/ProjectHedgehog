using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMixedDrink : MonoBehaviour
{

	public BarTenderHandler player;
	public DrinkManager dm;
	public GameObject panel;

	public void clicked() {
		Dictionary<string, MixedDrink> mixedDrinks = dm.mixedDrinks;
		string code = player.MakeTwoLetterSignature();
		Debug.Log(code);
		MixedDrink md;

		if (code == "") {
			return;
		}

		if (mixedDrinks.ContainsKey(code))
		{
			md = mixedDrinks[code];
			
		} else {
			md = mixedDrinks["Mystery"];
		}

		Transform quad = player.transform.Find("DrinkSprite");
        GameObject go2 = quad.gameObject;

        

        if (!go2.activeSelf)
        {
            go2.GetComponent<SpriteRenderer>().sprite =  Resources.Load<Sprite>($"Drinks/{md.sprite}");
            go2.GetComponent<DrinkHandler>().drink =  md;
			player.inventory = new List<Ingredient>();
            go2.SetActive(true);
            panel.SetActive(false);
        }
		

	}
}
