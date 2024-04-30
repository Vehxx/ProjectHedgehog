using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer : MonoBehaviour
{
    void Start()
    {
        Debug.Log(BarHandler.BarName);
        
        Debug.Log(BarHandler.KegCount);
        BarHandler.SellDrink();
        Debug.Log(BarHandler.KegCount);
    }
}
