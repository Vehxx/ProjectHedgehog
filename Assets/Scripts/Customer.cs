using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer
{
    public string name {get;set;}
    public int id {get;set;}

    public float startTime {get;set;} = 0;

    public float startDrink {get;set;} = 0;
    public float lastDrink {get;set;} = 0;

    public float drinkSpeed {get;set;} = 15;

    public float nightDrinkAffinity {get;set;} = 0.5f;
    public float overallDrinkAffinity {get;set;} = 1;
    
    public float getNewDrinkTime = 10; // Fix Starter Time
    
    public float waitTimeLimit {get;set;} = 30;
    
    public float nightSatisfaction {get;set;} = 0;
    public float overallSatisfaction {get;set;} = 0;
    
    public bool hasVisited {get;set;} = false;
    public bool hasDrink {get;set;} = false;
    public bool inChair = false;

    public float beerScore = 0.5f;

    public string reqDrink = null;
    
    public Customer(string name, int id)
    {
        this.name = name;
        this.id = id;
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

    public void setNewDrinkTime()
    {
        getNewDrinkTime = (60 * (1 - nightDrinkAffinity));
    }

    public bool readyToLeave() {
        return Mathf.Abs(nightSatisfaction) >= 3 && nightDrinkAffinity < 0.25;
    }

    public bool finishDrink(float time) {
        if ((time - startDrink) < drinkSpeed) 
        {
            return false;
        } else {
            hasDrink = false;
            lastDrink = time;
            setNewDrinkTime();
            return true;
        }
    }

    public float waitTimeCalculator(float time) {
        return (waitTimeLimit - (time - startTime))/waitTimeLimit;
    }



    
}
