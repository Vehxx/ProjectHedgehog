using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDay : MonoBehaviour
{

    public void clicked()
    {
        SceneManager.LoadScene("BeforeOpen",LoadSceneMode.Single);
    }
}