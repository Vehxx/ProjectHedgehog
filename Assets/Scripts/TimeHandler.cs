using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeHandler : MonoBehaviour
{
    public TMP_Text timeText;
    private int sceneStartTime = 0;
    private int time = 0;
    private int startTime = 8; 
    private int endTime = 14;
    private float gameTime = 2f;
    private bool isPaused = false;
	
    void Start()
    {
        sceneStartTime = (int) Time.time;
    }

    void Update()
    {
        if (!isPaused) {
            time = (int) Time.time - sceneStartTime;
        }

        timeText.text = getGameTime();
    }

    public string getGameTime() {
        int openTime = endTime - startTime;
        int openTimeSeconds = openTime * 60 * 60;
        int gameTimeSeconds = (int) (gameTime * 60);

        float increment = (float) openTimeSeconds / (float) gameTimeSeconds;
        float timePassed = (float) time * increment;

        int hours = (int) Mathf.Floor(timePassed / 3600);
        int minutes = ((int) timePassed / 60) % 60;

        string buildString = "";
        
        if ((startTime + hours) > 12) {
            buildString += (startTime + hours - 12).ToString();
        } else {
            buildString += (startTime + hours).ToString();
        }

        buildString += ":";
        if (minutes < 10) {
            buildString += "0";
        }
        buildString += minutes.ToString();
        if ((startTime + hours) >= 12) {
            buildString += " A.M.";
        } else {
            buildString += " P.M.";
        }

        return buildString;
    }

    public bool endDay() {
        int openTime = endTime - startTime;
        int openTimeSeconds = openTime * 60 * 60;
        int gameTimeSeconds = (int) (gameTime * 60);

        float increment = (float) openTimeSeconds / (float) gameTimeSeconds;
        float timePassed = (float) time * increment;

        int hours = (int) Mathf.Floor(timePassed / 3600);
        int minutes = ((int) timePassed / 60) % 60;

        return ((startTime+hours) >= endTime);
    }

    public bool newEvent(int lastTime, int incr) {
        int openTime = endTime - startTime;
        int openTimeSeconds = openTime * 60 * 60;
        int gameTimeSeconds = (int) (gameTime * 60);

        float increment = (float) openTimeSeconds / (float) gameTimeSeconds;
        float timePassed = (float) time * increment;

        int minutes = ((int) timePassed / 60);

        return ((minutes-lastTime) >= incr);
    }

    public int getTimePassed() {
        int openTime = endTime - startTime;
        int openTimeSeconds = openTime * 60 * 60;
        int gameTimeSeconds = (int) (gameTime * 60);

        float increment = (float) openTimeSeconds / (float) gameTimeSeconds;
        float timePassed = (float) time * increment;

        int minutes = ((int) timePassed / 60);

        return minutes;
    }
}
