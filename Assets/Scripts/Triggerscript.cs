using UnityEngine;

public class Triggerscript : MonoBehaviour
{

    public GameObject LapComplete;
    public GameObject HalfLapComplete;

    void OnTriggerEnter()
    {
        LapComplete.SetActive(true);
        HalfLapComplete.SetActive(false);
    }

}
