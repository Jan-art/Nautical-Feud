using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// This script control the switching of the input fields using the key "TAB".
public class FieldControl : MonoBehaviour {

EventSystem system;
public Selectable firstInput;
public Button submitBtn;
   //Initialize Event System
    void Start()
    {
        system = EventSystem.current;
        firstInput.Select();
    }

   //Register Key stroke
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift)) {
            Selectable previous = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if(previous!=null) {
                previous.Select();
                Debug.Log("Went Up");
            }
        }
        else if(Input.GetKeyDown(KeyCode.Tab)) {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if(next!=null) {
                next.Select();
                Debug.Log("Went down");
            }
        }else if(Input.GetKeyDown(KeyCode.Return)) {
          submitBtn.onClick.Invoke();
          Debug.Log("Btn Pressed");
        }
    }
}