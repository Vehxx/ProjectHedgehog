using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public class BarHandler
{
    public string BarName { get; set; } = "TestName";
    public float MoneyCount { get; set; } = 100;

    public BarHandler() {
        this.BarName = "TestName";
        this.MoneyCount = 100;
    }

    public BarHandler(string bn, int mc) {
        this.BarName = bn;
        this.MoneyCount = mc;
    }
    
    public void SellDrink(float sell_price)
    {
        MoneyCount += sell_price;
    }

    public void BuyBeer(float sell_price)
    {
        MoneyCount -= sell_price;
    }
}
