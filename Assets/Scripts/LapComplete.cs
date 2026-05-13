using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LapComplete : MonoBehaviour
{
    public GameObject LapTrigger;
    public GameObject HalfTrigger;

    public GameObject MinuteDisplay;
    public GameObject MillisecondDisplay;
    public GameObject SecondsDisplay;

    public GameObject LaptimeBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter()
    {
        if (TimerManager.SecondsTimer <= 9)
        {
            SecondsDisplay.GetComponent<Text>().text = "0" + TimerManager.SecondsTimer + ".";
        }
        else
        {
            SecondsDisplay.GetComponent<Text>().text = "" + TimerManager.SecondsTimer + ".";
        }
        if (TimerManager.MinuteTimer <= 9)
        {
            MinuteDisplay.GetComponent<Text>().text = "0" + TimerManager.MinuteTimer + ".";
        }
        else
        {
            MinuteDisplay.GetComponent<Text>().text = "" + TimerManager.MinuteTimer + ".";

        }
        MillisecondDisplay.GetComponent<Text>().text = "" + TimerManager.MillisecondTimer;

        TimerManager.MinuteTimer = 0;
        TimerManager.SecondsTimer = 0;
        TimerManager.MillisecondTimer = 0;

        HalfTrigger.SetActive(true);
        LapTrigger.SetActive(false);
    }
}
