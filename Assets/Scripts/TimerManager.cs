using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static int MinuteTimer;
    public static int SecondsTimer;
    public static float MillisecondTimer;
    public static string MillisecondDisplay;

    //Add the gameobjects so that I can add the gameobjects in unity//
    public GameObject Minutebox;
    public GameObject Secondsbox;
    public GameObject MillisecondBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Deltatime is less than milliseconds so multiply by 10 to get milliseconds//
        MillisecondTimer += Time.deltaTime * 10;
        MillisecondDisplay = MillisecondTimer.ToString("F0");
        MillisecondBox.GetComponent<Text>().text = "" + MillisecondDisplay;

        if (MillisecondTimer >= 10)
        {
            //When Milliseconds hit max, converts it to a 1 on the seconds display.//
            MillisecondTimer = 0;
            SecondsTimer += 1;
        }

        if (SecondsTimer <= 9)
        {
            //If the seconds are less than 10, it'll be like 09 if its 9 or 08. Otherwise its just displaying the full number//
            Secondsbox.GetComponent<Text> ().text = "0" + SecondsTimer + ".";
        }
        else
        {
            Secondsbox.GetComponent<Text> ().text = "" + SecondsTimer + ".";
        }
        if(SecondsTimer >= 60)
        {
            SecondsTimer = 0;
            MinuteTimer += 1;
        }
        if (MinuteTimer <= 9)
        {
            //If the seconds are less than 10, it'll be like 09 if its 9 or 08. Otherwise its just displaying the full number//
            Minutebox.GetComponent<Text> ().text = "0" + MinuteTimer + ":";
        }
        else
        {
            Minutebox.GetComponent<Text> ().text = "" + MinuteTimer + ":";
        }
    }
}
