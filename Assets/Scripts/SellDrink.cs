using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellDrink : MonoBehaviour
{
    public GameHandler gh;
    public GameObject player;
    public GameObject customerButton;
    public TimeHandler th;
    public float interactDistance = 3f;
    public bool revealed = false;

    public void clicked()
    {
        CustomerHandler customerHandler = customerButton.GetComponentInChildren<CustomerHandler>(true);
        if (!revealed) {
            Transform reqDrinkT = customerButton.transform.Find("DrinkRequest");
            GameObject reqDrinkGO = reqDrinkT.gameObject;

            var beer = reqDrinkGO.GetComponent<DrinkHandler>().beer;
            var md = reqDrinkGO.GetComponent<DrinkHandler>().drink;

            if (beer != null) {
                reqDrinkGO.GetComponent<SpriteRenderer>().sprite =  Resources.Load<Sprite>($"Drinks/{beer.logo_sprite}");
            } else {
                reqDrinkGO.GetComponent<SpriteRenderer>().sprite =  Resources.Load<Sprite>($"Drinks/{md.sprite}");
            }
            revealed = true;

        } else if (!customerHandler.customer.hasDrink) {
            Transform reqDrinkT = customerButton.transform.Find("DrinkRequest");
            GameObject reqDrinkGO = reqDrinkT.gameObject;
  
            Transform drinkSpriteT = player.transform.Find("DrinkSprite");
            GameObject drinkSpriteGO = drinkSpriteT.gameObject;

            Vector3 p = drinkSpriteT.position;
            Vector3 t = reqDrinkT.position;

            float sqrDist = (p - t).sqrMagnitude;
            float sqrRange = interactDistance * interactDistance;

            Transform quad = player.transform.Find("DrinkSprite");
            GameObject go2 = quad.gameObject;

            var beer = go2.GetComponent<DrinkHandler>().beer;
            var md = go2.GetComponent<DrinkHandler>().drink;

            if (beer == null && md == null) {
                return;
            }
        

            if (sqrDist > sqrRange)
            {
                // Too far: early out (optional: play a "too far" sound/UI hint)
                // Debug.Log($"Too far to draft. Need < {interactDistance:F2}, have {Mathf.Sqrt(sqrDist):F2}");
                return;
            }

            if (drinkSpriteGO.activeSelf && reqDrinkGO.activeSelf)
            {
                drinkSpriteGO.SetActive(false);
                //reqDrinkGO.SetActive(false);

                bool checkHappy = true;
               
                float waitTimeFactor = customerHandler.customer.waitTimeCalculator(th.getTimePassed());
                customerHandler.customer.nightSatisfaction += waitTimeFactor;
                if (waitTimeFactor < -1.5) {
                    checkHappy = false;
                } 

                if (beer != null) {
                    gh.bh.SellDrink(beer.sell_price);
                    if (customerHandler.customer.reqDrink ==  go2.GetComponent<DrinkHandler>().beer.id) {
                        customerHandler.customer.nightSatisfaction += 1;
                        customerHandler.customer.nightDrinkAffinity *= 0.25f;
                    } else {
                        customerHandler.customer.nightSatisfaction -= 1;
                        customerHandler.customer.nightDrinkAffinity *= 0.25f;
                        checkHappy = false;
                    } 
                    go2.GetComponent<DrinkHandler>().beer = null;
                } else {
                    gh.bh.SellDrink(md.sell_price);
                    if (customerHandler.customer.reqDrink ==  go2.GetComponent<DrinkHandler>().drink.id) {
                        customerHandler.customer.nightSatisfaction += 1;
                        customerHandler.customer.nightDrinkAffinity *= 0.25f;
                    } else {
                        customerHandler.customer.nightSatisfaction -= 1;
                        customerHandler.customer.nightDrinkAffinity *= 0.25f;
                        checkHappy = false;
                    }
                    go2.GetComponent<DrinkHandler>().drink = null;
                }

                if (checkHappy) {
                    reqDrinkGO.GetComponent<SpriteRenderer>().sprite =  Resources.Load<Sprite>($"Drinks/HAPPY");
                } else {
                    reqDrinkGO.GetComponent<SpriteRenderer>().sprite =  Resources.Load<Sprite>($"Drinks/ANGRY");
                }



            
                customerHandler.customer.startDrink = th.getTimePassed();
                customerHandler.customer.reqDrink = null;
                customerHandler.customer.hasDrink = true;

            }
            
        } else {

        }
    }
}
