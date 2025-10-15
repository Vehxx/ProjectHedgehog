using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SetBottlePanel : MonoBehaviour
{
    public GameObject panel;
    public string txt;
    
    public void clicked()
    {
        panel.GetComponent<IngredientPanel>().type = txt;
    }
}
