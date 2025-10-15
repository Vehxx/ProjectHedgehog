using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DraftBeer : MonoBehaviour
{
    public GameObject player;
    public GameObject button;
    public GameObject panel;

    public void clicked()
    {
        Beer beer = button.GetComponentInChildren<DrinkHandler>(true).beer;

        if (beer.amount < 1)
        {
            return;
        }
   
        Transform quad = player.transform.Find("DrinkSprite");
        GameObject go2 = quad.gameObject;

        if (!go2.activeSelf)
        {
            go2.GetComponent<SpriteRenderer>().sprite =  Resources.Load<Sprite>($"Drinks/{beer.drink_sprite}");
            go2.GetComponent<DrinkHandler>().beer =  beer;
            go2.SetActive(true);
            panel.SetActive(false);
            beer.amount --;
        }
    }
}