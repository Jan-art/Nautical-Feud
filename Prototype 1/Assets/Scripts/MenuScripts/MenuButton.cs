using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//This script controls the "select" and "pressed" animations for the menu buttons.

public class MenuButton : MonoBehaviour
{
	[SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;
	[SerializeField] int thisIndex;

	// Update is called once per frame
	void Update()
	{
		//Bugged - Highlights all buttons as active, should in theory only be triggered by the buttons it is hovering over.
		if (EventSystem.current.IsPointerOverGameObject())
		{
			Debug.Log("EventSystem triggered");
			menuButtonController.mousePressed(thisIndex);
		}
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

	void setPressedFalse()
    {
		animator.SetBool("pressed", false);
    }




	void OnMouseEnter()
	{
		Debug.Log("OnMouseOver running");
		menuButtonController.mousePressed(thisIndex);
	}
}
