using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using TMPro;

public class BeforeOpenManager : MonoBehaviour
{
    public BarHandler bh {get;set;} = new BarHandler();
    public GameDataManager gdm;
    public DrinkManager dm;
    public bool startDay {get;set;} = false;
    public TMP_Text goldText;

    void Start()
    {
        bh = gdm.loadBarStatus();
    }

    void Update()
    {
        if (startDay) { 
            gdm.updateBarStatus(bh);
            gdm.SaveBeers(dm.beers);
            gdm.SaveIngredients(dm.ingredients);
            SceneManager.LoadScene("Tavern",LoadSceneMode.Single);
        }

        goldText.text = "Gold : " + bh.MoneyCount;

        
    }
}
