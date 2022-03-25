using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

//Auth System

public class AuthSystem : MonoBehaviour
{

    public InputField user;
    public InputField mail;
    public InputField pass;

    public bool IsAuthenticated = false;

    LoginWithPlayFabRequest loginRequest;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Login()
    {
       loginRequest = new LoginWithPlayFabRequest();
       loginRequest.Username = user.text;
       //loginRequest.Email = mail.text;
       loginRequest.Password = pass.text;
       PlayFabClientAPI.LoginWithPlayFab(loginRequest, result => {
           //If user account is found
           IsAuthenticated = true;
           Debug.Log("Logged-in !");
       }, error => {
           //If user account is not found
           IsAuthenticated = false;
           Debug.Log(error.ErrorMessage);

       }, null);
    }

    public void Register()
    {

    }
}
