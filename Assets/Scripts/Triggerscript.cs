using UnityEngine;

public class Triggerscript : MonoBehaviour
{

    public GameObject LapComplete;
    public GameObject HalfLapComplete;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LapComplete.SetActive(true);
            HalfLapComplete.SetActive(false);
        }
    }

}
