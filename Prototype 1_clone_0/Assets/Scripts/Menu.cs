using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("Menu Scene Loaded");
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
