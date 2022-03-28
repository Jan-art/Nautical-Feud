using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvanceMC : MonoBehaviour
{
    [SerializeField]
    GameObject AdvModeCheck;

    private bool AMC;
    private string username;

    private static AdvanceMC instance;

    void Awake()
    {
        gameObject.tag = "AdvModeCheck";
        DontDestroyOnLoad(AdvModeCheck);
        if (instance == null)
            instance = this;
        else
            Destroy(this.AdvModeCheck);
    }
    // Start is called before the first frame update
    void Start()
    {
        AMC = false;
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

    public void setUsername(string _username)
    {
        username = _username;
    }

    public string getUsername()
    {
        return username;
    }
}
