using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public static bool GameActive = false;
    public GameObject PauseCanvas;
    // public GameObject

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameActive)
            {
                RestartGame();
                Debug.Log("Game UnPaused");
            } else
            {
                PauseGame();
                Debug.Log("Game Paused");
            }
        }

    }

    public void RestartGame()
    {
        PauseCanvas.SetActive(false);
        
        GameActive = false;
    }

    void PauseGame()
    {
        PauseCanvas.SetActive(true);

        
        GameActive = true;
    }
}
