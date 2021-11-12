using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject PlayBtn;
    [SerializeField]
    GameObject Searching;

    void Start()
    {
        Searching.SetActive(false);
        PlayBtn.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Passed 'ConnectUsingSettings()' method");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to photon on " + PhotonNetwork.CloudRegion +  " server.");
        PhotonNetwork.AutomaticallySyncScene = true;
        PlayBtn.SetActive(true);
    }

    public void FindMatch()
    {
        Searching.SetActive(true);
        PlayBtn.SetActive(false);
    }

    public void StopSearch()
    {
        Searching.SetActive(false);
        PlayBtn.SetActive(true);
    }
}
