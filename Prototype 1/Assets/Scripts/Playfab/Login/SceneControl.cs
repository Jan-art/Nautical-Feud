using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public void MoveToRegister()
    {
        SceneManager.LoadScene("Register.Scene");
        Debug.Log("Register Scene Loaded");
    }

    public void MoveToLogin()
    {
        SceneManager.LoadScene("Login.Scene");
        Debug.Log("Login Scene Loaded");
    }


}
