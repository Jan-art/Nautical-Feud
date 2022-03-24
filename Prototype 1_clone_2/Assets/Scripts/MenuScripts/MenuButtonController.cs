using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//This script controls the keyboard input for the Menu animations

public class MenuButtonController : MonoBehaviour
{

//Initialization
	public int index;
	[SerializeField] bool keyDown;
	[SerializeField] int maxIndex;
	public AudioSource audioSource;

	void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

		//Keyboard Input (Arrow Up/Down)

		if(Input.GetAxis ("Vertical") != 0){ 
			if(!keyDown){
				if (Input.GetAxis ("Vertical") < 0) {
					if(index < maxIndex){
						index++;
					}else{

						index = 0;
					}
				} else if(Input.GetAxis ("Vertical") > 0){
					if(index > 0){
						index --; 
					}else{
						index = maxIndex;
					}
				}
				keyDown = true;
			}
		}else{
			keyDown = false;
		}
    
		//Mouse Input...


	}

	public void mousePressed(int buttonIndex)
	{
		index = buttonIndex;
	}


}
