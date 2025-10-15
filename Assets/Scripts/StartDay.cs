using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartDay : MonoBehaviour
{
    public BeforeOpenManager bom;

    public void clicked()
    {
        bom.startDay = true;
    }
}