using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Menu Scene Loaded");
    }

    public void NextSlide()
    {
        SceneManager.LoadScene("Tutorial2.Scene");
        Debug.Log("Slide 2 Loaded");
    }

    public void PreviousSlide()
    {
          SceneManager.LoadScene("Tutorial.Scene");
          Debug.Log("Slide 1 Loaded");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restart Scene Loaded");
    }

    public void Load(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("New Scene Loaded");

    }

    public void ExitGame()
    {
     Application.Quit();
     Debug.Log("Game is exiting");
    }
}
