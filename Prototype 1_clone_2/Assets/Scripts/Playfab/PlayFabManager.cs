using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

//Auth System

public class PlayFabManager : MonoBehaviour
{
    public GameObject rowPrefab; //The Row Prefab which contain all text boxes to display the data
    public Transform rowsParent; // Paren GameObt for RowPrefab

    [Header("UI")]
    public Text messageText;
    public InputField mail;
    public InputField pass;

    public GameObject AdvModeCheck;
    public GameObject usernamePopUp;
    public InputField user;

    void Awake()
    {
        AdvModeCheck = GameObject.FindGameObjectWithTag("AdvModeCheck");
    }

    public void start()
    {
        GetLeaderboard();
    }

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
        createStatistics();
        enableUsernamePanel();
    }

    public void enableUsernamePanel()
    {
        usernamePopUp.SetActive(true);
    }

    public void UsernameBtn()
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest();
        request.DisplayName = user.text;
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnNameUpdated, OnError);
    }

    void OnNameUpdated(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Account username set");
        AdvModeCheck.GetComponent<AdvanceMC>().setUsername(user.text);
        usernamePopUp.SetActive(false);
    }

    //==============================================================
    //    STATS CREATED FOR USER AFTER LOGIN
    //==============================================================

    public void createStatistics()
    {
        var requestWins = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate
                        {
                            StatisticName = "Matches Won",
                            Value = 0
                        }
                    }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(requestWins, OnLeaderboardUpdate, OnError);

        var requestLosses = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "Matches Lost",
                        Value = 0
                    }
                }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(requestLosses, OnLeaderboardUpdate, OnError);

        var requestShipsSunk = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate
                        {
                            StatisticName = "Ships Sunk",
                            Value = 0
                        }
                    }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(requestShipsSunk, OnLeaderboardUpdate, OnError);
    }


    //=============================================================
    public void LogBtn()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = mail.text,
            Password = pass.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        messageText.text = "Logged-in ! ";
        Debug.Log("Account creation Success");
        AdvModeCheck.GetComponent<AdvanceMC>().setUsername(result.InfoResultPayload.PlayerProfile.DisplayName);
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

    //==============================================================
    //    FUNCTIONS TO SEND STATS TO GAME-BOARD
    //==============================================================

    public void SendLeaderboard(int Wins)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Matches Won",
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

   
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Matches Won",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }



    // ATTEMP 1.0 AT SENDING DATA @Daniel
    //(From My Understanding Something similar to the script below needs to be done with the script you wrote in GameManager) @Daniel

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }


        foreach (var item in result.Leaderboard)
        {
            GameObject newObj = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newObj.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.PlayFabId.ToString();
            texts[2].text = item.StatValue.ToString();

            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }
    }
    
}
