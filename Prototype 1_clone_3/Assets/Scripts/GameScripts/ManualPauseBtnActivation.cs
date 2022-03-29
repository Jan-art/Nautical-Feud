using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script controls the onClick event for the Pause Icon BTN
public class ManualPauseBtnActivation : MonoBehaviour
{
    private bool ActiveState;
    public GameObject PauseMenu;

    // Start is called before the first frame update
    void Awake()
    {
        PauseMenu.SetActive(false);
        ActiveState = false;
    }


    public void Toggle()
    {
        Debug.Log("Function Toggle ran");
        if (ActiveState == false)
        {
            PauseMenu.SetActive(true);
            ActiveState = true;
            Debug.Log("PauseMenu set to active");
        }
        else if (ActiveState == true)
        {
            PauseMenu.SetActive(false);
            ActiveState = false;
            Debug.Log("PauseMenu set to inactive");
        }
    }
}
