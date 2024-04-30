using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class BarHandler
{
    public static string BarName { get; set; } = "TestName";
    public static int KegCount { get; set; } = 100;
    
    public static void SellDrink()
    {
        KegCount--;
    }
}
