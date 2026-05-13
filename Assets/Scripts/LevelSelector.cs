using UnityEngine;
using System.Collections; 
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OpenScene()
    {
        SceneManager.LoadSceneAsync("Track 1");
    }
}
