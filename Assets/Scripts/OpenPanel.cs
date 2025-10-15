using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{

    public GameObject player;
    public GameObject bartap;
    public GameObject panel;    
    public float interactDistance = 5f;

    public void clicked()
    {
        Transform btGOT = bartap.transform;
        GameObject btGO = bartap;
  
        Transform drinkSpriteT = player.transform.Find("DrinkSprite");
        GameObject drinkSpriteGO = drinkSpriteT.gameObject;

        Vector3 p = drinkSpriteT.position;
        Vector3 t = btGOT.position;

        float sqrDist = (p - t).sqrMagnitude;
        float sqrRange = interactDistance * interactDistance;

        if (sqrDist > sqrRange)
        {
            // Too far: early out (optional: play a "too far" sound/UI hint)
            // Debug.Log($"Too far to draft. Need < {interactDistance:F2}, have {Mathf.Sqrt(sqrDist):F2}");
            return;
        }

        panel.SetActive(true);
        player.GetComponentInChildren<BarTenderHandler>().panel = panel;
    }
    

}
