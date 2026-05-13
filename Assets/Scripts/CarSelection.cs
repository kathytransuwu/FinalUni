using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelection : MonoBehaviour
{

    public GameObject[] cars;

    public Button next;

    public Button previous;

    int index;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        index = PlayerPrefs.GetInt("CarSelected");

        for(int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(false);
            cars[index].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(index >= 2)
        {
           next.interactable = false;
        }
        else
        {
            next.interactable = true;
        }
        if(index <= 0)
        {
            previous.interactable = false;
        }
        else
        {
            previous.interactable = true;
        }
    }

    public void Next()
    {
        index++;

        for(int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(false);
            cars[index].SetActive(true);
        }

        PlayerPrefs.SetInt("CarSelected", index);
        PlayerPrefs.Save();
    }

    public void Previous()
    {
        index--;
        for(int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(false);
            cars[index].SetActive(true);
        }
        PlayerPrefs.SetInt("CarSelected", index);
        PlayerPrefs.Save();
    }

    public void Race()
    {
        SceneManager.LoadSceneAsync("CourseSelection");
    }
}
