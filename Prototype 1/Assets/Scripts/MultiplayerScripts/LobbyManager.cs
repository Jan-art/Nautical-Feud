using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject MainMenu;
    [SerializeField]
    GameObject Searching;
    [SerializeField]
    GameObject AdvModeCheck;

    public const string ADVANCED_MODE = "adv";

    void Start()
    {
        Searching.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            MainMenu.SetActive(true);
        }
        else
        {
            MainMenu.SetActive(false);
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    //On connection to server reveals the play button to allow players to search for matches
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to photon on " + PhotonNetwork.CloudRegion + " server.");
        PhotonNetwork.AutomaticallySyncScene = true;
        MainMenu.SetActive(true);
    }

    public void FindMatch()
    {
        Searching.SetActive(true);
        MainMenu.SetActive(false);

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
        if (AdvModeCheck.GetComponent<AdvanceMC>().getAMC())
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
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
    }

    //Starts the match once another player enters the room
    public override void OnPlayerEnteredRoom(Player secondPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
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

}
