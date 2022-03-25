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
    public Text msg;

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
       loginRequest.Password = pass.text;

       mail.gameObject.SetActive(false);
       PlayFabClientAPI.LoginWithPlayFab(loginRequest, result => {
           //If user account is found
           IsAuthenticated = true;
           msg.text = user.text + "\n" + " You have logged-in successfuly ! "; 
           Debug.Log("Logged-in !");
       }, error => {
           //If user account is not found
           IsAuthenticated = false;
           msg.text = "Failed to Login [" + error.ErrorMessage + "]";
           Debug.Log(error.ErrorMessage);

       }, null);
    }

    public void Register()
    {
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
        request.Email = mail.text;
        request.Username = user.text;
        request.Password = pass.text;

        PlayFabClientAPI.RegisterPlayFabUser(request, result => 
        {
            msg.text = "Account Registered ";
            Debug.Log("Account Created!");
        }, error=>
        {
            msg.text = "Failed to create account ["+error.ErrorMessage+"]";
            Debug.Log("Failed to create an account");
        });
    }
}
