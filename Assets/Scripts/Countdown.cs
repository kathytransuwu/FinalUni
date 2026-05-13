using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
public class Countdown : MonoBehaviour
{
    public TextMeshProUGUI CountdownText;

    private int countdownValue = 3;

    public GameObject LapTimer;

    public GameObject CarControls;

    private bool CarStarted = false;

    private bool Lapstarted = false;

    public AudioSource CountdownBeep;

    public AudioSource CountdownFinalBeep;



    void Start()
    {
        StartCoroutine(CountdownTimer());
        CarStarted = CarControls.GetComponent<Carcontroller>().enabled = false;
        Lapstarted = LapTimer.GetComponent<TimerManager>().enabled = false; //Disables the car controls and the timer at the start of the game

    }
    
    IEnumerator CountdownTimer()
    {
        while (countdownValue >= 0)
        {
            CountdownBeep.Play(); //Plays the beep sound every second, spliced it into two sounds so that the beeps are in sync with the countdown//
            Console.WriteLine(countdownValue);
            CountdownText.text = countdownValue.ToString();
            yield return new WaitForSeconds(1f);
            countdownValue--;
        }
        CountdownFinalBeep.Play();
        Countdown.Destroy(CountdownFinalBeep, 1f);
        CarStarted = CarControls.GetComponent<Carcontroller>().enabled = true; //Enables the car controls and the timer when the countdown hits 0//
        Lapstarted = LapTimer.GetComponent<TimerManager>().enabled = true;
        CountdownText.text = "Start!~";
        Countdown.Destroy(CountdownText, 1f); //Destroys the countdown text 1 second after the countdown hits 0//



    }
}
