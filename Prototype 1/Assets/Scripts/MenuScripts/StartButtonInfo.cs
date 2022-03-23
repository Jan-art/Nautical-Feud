using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartButtonInfo : MonoBehaviour
{
    public GameObject LobbyManager;
    public bool advancedMode;

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                LobbyManager.GetComponent<LobbyManager>().setAdvancedMode(advancedMode);
            }
        }
    }
}
