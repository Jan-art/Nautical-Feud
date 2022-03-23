using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvanceMC : MonoBehaviour
{
    [SerializeField]
    GameObject AdvModeCheck;

    private bool AMC;

    // Start is called before the first frame update
    void Start()
    {
        AMC = true;
        DontDestroyOnLoad(AdvModeCheck);
        gameObject.tag = "AdvModeCheck";
    }

    public void setAMC(bool boolTF)
    {
        if (boolTF == true)
        {
            AMC = true;
        }
        else if (boolTF == false)
        {
            AMC = false;
        }
        else
        {
            Debug.Log("Else statement hit in AdvanceMC script on AdvModeCheck GameObject. This is an error");
        }
    }

    public bool getAMC()
    {
        return AMC;
    }
}
