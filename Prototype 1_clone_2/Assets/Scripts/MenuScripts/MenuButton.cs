using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//This script controls the "select" and "pressed" animations for the menu buttons.

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	[SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;
	[SerializeField] int thisIndex;
	[SerializeField] bool cursorOnElement;

	public GameObject LobbyManager;

	// Update is called once per frame
	void Update()
	{
		//If the cursor is not on the element check whether the button should be highlighted
		if (!cursorOnElement || animator.GetBool("pressed"))
		{
			if (menuButtonController.index == thisIndex)
			{
				animator.SetBool("selected", true);
				if (Input.GetAxis("Submit") == 1)
				{
					animator.SetBool("pressed", true);
				}
				else if (animator.GetBool("pressed"))
				{
					animator.SetBool("pressed", false);
					animatorFunctions.disableOnce = true;
				}
			}
			else
			{
				animator.SetBool("selected", false);
			}
		}
        else
        {
			animator.SetBool("selected", true);
        }
	}

	//When a cursor enter the object
	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("The cursor entered the selectable UI element.");
		animator.SetBool("selected", true);
		cursorOnElement = true;
		menuButtonController.index = thisIndex;
	}

	//When a cursor presses on the object
	public void OnPointerDown(PointerEventData eventData)
    {
		//Doesn't play animation like pressing enter via keyboard does
		Debug.Log("The cursor was pressed");
		animator.SetBool("pressed", true);

		if (thisIndex == 0)
        {
			LobbyManager.GetComponent<LobbyManager>().setAdvancedMode(false);
		}
		else if (thisIndex == 1)
        {
			LobbyManager.GetComponent<LobbyManager>().setAdvancedMode(true);
		}
	}

	//When a pointer exits the object
	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("The cursor has left the UI element");
		cursorOnElement = false;
    }

}
