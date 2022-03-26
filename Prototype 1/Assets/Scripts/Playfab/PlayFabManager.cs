using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

//Auth System

public class PlayFabManager : MonoBehaviour
{

    [Header("UI")]
    public Text messageText;
    public InputField mail;
    public InputField pass;

    

//=============================================================
    public void RegBtn()
    {
        if (pass.text.Length < 6)
        {
            messageText.text = "Password is too short!";
            return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            Email = mail.text,
            Password = pass.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);

    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Registered & Logged in";
    }

 //=============================================================
    public void LogBtn()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = mail.text,
            Password = pass.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        messageText.text = "Logged-in ! ";
        Debug.Log("Account creation Success");

    }

    //=============================================================

    public void ResetPassBtn()
    {
        var request = new SendAccountRecoveryEmailRequest {
            Email = mail.text,
            TitleId = "412BB"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);  
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
     messageText.text = "Check your mail, reset link sent!";
     Debug.Log("Reset Link Sent");
    }

//=============================================================

    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    //=============================================================

    /*
    public void SendLeaderboard(int Wins)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Wins",
                    Value = Wins

                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);

    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Leaderboard Sent");
    }

   /*ublic void Defeat()
    {
        //GameOverText.SetActive(true);
        //PlayfabManager.SendLeaderboard(maxPlatform);
    }
   
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Wins",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }
    }
    */
}
