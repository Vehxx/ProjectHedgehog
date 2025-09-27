using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer
{
    private string name;
    private int id;
    private float startTime = 0;
    private float lastDrink = 0;
    private float getNewDrinkTime = 20;
    
    public Customer(string name, int id)
    {
        this.name = name;
        this.id = id;
    }

    public string getFirstName()
    {
        return this.name;
    }

    public float getStartTime()
    {
        return this.startTime;
    }

    public void setStartTime(float time)
    {
        startTime = time;
    }

    public float getLastDrinkTime()
    {
        return this.lastDrink;
    }

    public void setDrinkTime(float time)
    {
        lastDrink = time;
    }

    public bool checkLastDrink(float time)
    {
        if ((time - lastDrink) < getNewDrinkTime) 
        {
            return false;
        } else {
            return true;
        }
    }
}
