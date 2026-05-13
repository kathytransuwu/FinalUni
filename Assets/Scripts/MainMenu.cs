using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void StartGame()
    {
        // Load the scene with index 1, which is typically the main game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}
