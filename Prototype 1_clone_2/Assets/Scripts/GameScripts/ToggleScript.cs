using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScript : MonoBehaviour
{
    private bool toggleState;
    public GameObject PowerUpBar;
    // Start is called before the first frame update
    void Awake()
    {
        PowerUpBar.SetActive(false);
        toggleState = false;
    }

    void Start()
    {
    }

    public void Toggle()
    {
        Debug.Log("Function Toggle ran");
        if (toggleState == false)
        {
            PowerUpBar.SetActive(true);
            toggleState = true;
            Debug.Log("PowerUpBar set to active");
        }
        else if (toggleState == true)
        {
            PowerUpBar.SetActive(false);
            toggleState = false;
            Debug.Log("PowerUpBar set to inactive");
        }
    }
}
