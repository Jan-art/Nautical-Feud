using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallLeaderBoard : MonoBehaviour
{

    public GameObject PlayFabManager;
    public GameObject AdvModeCheck;
    public GameObject LogIn;

    void Awake()
    {
        AdvModeCheck = GameObject.FindGameObjectWithTag("AdvModeCheck");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (AdvModeCheck.GetComponent<AdvanceMC>().getUsername() != null)
        {
            PlayFabManager.GetComponent<PlayFabManager>().GetLeaderboard();
        }
        else
        {
            LogIn.SetActive(true);
        }
    }

}
