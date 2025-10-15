using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using TMPro;


public class GameHandler : MonoBehaviour
{
	public TimeHandler th;
    public BarHandler bh {get;set;} = new BarHandler();
    public GameDataManager gdm;
    public DrinkManager dm;
    public TMP_Text goldText;

    void Start()
    {
        bh = gdm.loadBarStatus();
    }

    void Update()
    {
        if (th.endDay()) { 
            gdm.updateBarStatus(bh);
            gdm.SaveBeers(dm.beers);
            gdm.SaveIngredients(dm.ingredients);
            SceneManager.LoadScene("AfterClose",LoadSceneMode.Single);
        }

        goldText.text = "Gold : " + bh.MoneyCount;

        
    }
}