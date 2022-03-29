using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject MainMenu;
    [SerializeField]
    GameObject Searching;
    [SerializeField]
    GameObject AdvModeCheck;

    public Text SearchingTxt;
    public Text welcomeText;
    public GameObject LoginBtn;
    public GameObject LogoutBtn;

    public const string ADVANCED_MODE = "adv";

    void Start()
    {
        string randomName = $"Tester{Guid.NewGuid().ToString()}";

        AdvModeCheck = GameObject.FindGameObjectWithTag("AdvModeCheck");
        Searching.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            MainMenu.SetActive(true);

        }
        else
        {
            MainMenu.SetActive(false);
        }
        if (AdvModeCheck.GetComponent<AdvanceMC>().getUsername() != null)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues(AdvModeCheck.GetComponent<AdvanceMC>().getUsername());
            PhotonNetwork.NickName = AdvModeCheck.GetComponent<AdvanceMC>().getUsername();
            Debug.Log("Photon Network nickname set");
            LoginBtn.SetActive(false);
            LogoutBtn.SetActive(true);
        }
        else
        {
            ConnectToPhoton(randomName);

        }
    }


    public void ConnectToPhoton(string nickName)
    {
        Debug.Log($" as : { nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.NickName = nickName;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }


    }
    //On connection to server reveals the play button to allow players to search for matches
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to photon on " + PhotonNetwork.CloudRegion + " server." );
       
        PhotonNetwork.AutomaticallySyncScene = true;
       
        MainMenu.SetActive(true);
    }

    public void FindMatch()
    {
        Searching.SetActive(true);
        MainMenu.SetActive(false);
        SearchingTxt.text = "S E A R C H I N G   F O R   A   R A N D O M   R O O M";
        if (AdvModeCheck.GetComponent<AdvanceMC>().getAMC())
        {
            Debug.Log("Joining random room with advanced mode active");
            Hashtable expectedCustomRoomProperties = new Hashtable { { "ADVANCED_MODE", 1 } };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        }
        else
        {
            Debug.Log("Joining random room with classic mode active");
            Hashtable expectedCustomRoomProperties = new Hashtable { { "ADVANCED_MODE", 0 } };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        }
        Debug.Log("Searching for a game");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Couldn't find room. Creating own room");
        SearchingTxt.text = "R A N D O M   R O O M   N O T   F O U N D";
        MakeRoom();
    }

    //Method to create a room
    public void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);

        //Makes a room for two players that can be joined by anyone
        RoomOptions roomOptions =
        new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "ADVANCED_MODE" };
        if (AdvModeCheck.GetComponent<AdvanceMC>().getAMC() == true)
        {
            roomOptions.CustomRoomProperties = new Hashtable { { "ADVANCED_MODE", 1 } };
            Debug.Log("Made custom room with property with advanced mode key linked to true");
        }
        else
        {
            roomOptions.CustomRoomProperties = new Hashtable { { "ADVANCED_MODE", 0 } };
            Debug.Log("Made custom room with property with advanced mode key linked to false");
        }
        PhotonNetwork.CreateRoom("RoomName_" + randomRoomName, roomOptions);
        Debug.Log("Room created. Waiting for another player...");
        SearchingTxt.text = "R O O M   C R E A T E D   W A I T I N G   F O R   O T H E R   P L A Y E R";
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //Starts the match once another player enters the room
    public override void OnPlayerEnteredRoom(Player secondPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            SearchingTxt.text = "S T A R T I N G   G A M E";
            Debug.Log("Starting match");
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void StopSearch()
    {
        Searching.SetActive(false);
        MainMenu.SetActive(true);
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LeaveRoom();
        Debug.Log("Search Stopped");
    }

    public void setAdvancedMode(bool adv)
    {
        AdvModeCheck.GetComponent<AdvanceMC>().setAMC(adv);
        FindMatch();
    }

    public void LogoutBtnPressed()
    {
        Debug.Log("You have been logged out");
        AdvModeCheck.GetComponent<AdvanceMC>().setUsername(null);
        welcomeText.text = "Welcome \nGuest!";
        LoginBtn.SetActive(true);
        LogoutBtn.SetActive(false);
        string randomName = $"Tester{Guid.NewGuid().ToString()}";
        ConnectToPhoton(randomName);
    }

}
