using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using Photon.Pun;

//This Script Manages ChatBox

public class ChatManager : MonoBehaviour
{
    public TMP_InputField MsgInput;
    public TextMeshProUGUI MsgContent;

    private PhotonView _photon;

    private List<string> _messages = new List<string>();
    private float _CreationDelay = 0f;
    private int _maxMsg = 10;

    void Start()
    {
        _photon = GetComponent<PhotonView>();
    }

    [PunRPC]
    void RPC_AddNewMessage(string msg)
    {
        _messages.Add(msg);
    }

    public void SendChat(string msg)
    {
         string NewMessage = PhotonNetwork.NickName + ": " + "\n" + msg;
        _photon.RPC("RPC_AddNewMessage", RpcTarget.All, NewMessage);
    }

    public void SubmitChat()
    {
        string emptyCheck = MsgInput.text;
        emptyCheck = Regex.Replace(emptyCheck, @"\s", "");
        if(emptyCheck == "")
        {
            MsgInput.ActivateInputField();
            MsgInput.text = "";
            return;
        }

        SendChat(MsgInput.text);
        MsgInput.ActivateInputField();
        MsgInput.text = "";
    }

    void createChatBoxDisplay()
    {
        string NewContent = "";
        foreach(string s in _messages)
        {
            NewContent += s + "\n";
        }
        MsgContent.text = NewContent;
    }

    void Update()
    {
       if(PhotonNetwork.InRoom)
       {
            MsgContent.maxVisibleLines = _maxMsg;

            if(_messages.Count > _maxMsg)
            {
                _messages.RemoveAt(0);
            }
            if(_CreationDelay < Time.time)
            {
                createChatBoxDisplay();
                _CreationDelay = Time.time + 0.20f;
            }
       }
       else if(_messages.Count > 0)
       {
            _messages.Clear();
            MsgContent.text = "";
       }
    }
}
