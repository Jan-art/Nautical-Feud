using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


//This script will be used to load the relative screen on the press of a button.

public class SceneLoader : MonoBehaviour
{
 
 public void LoadScene()
 {
  SceneManager.LoadScene("Test"); //FIND A WAY TO MAKE THE BUTTONS WORK WITH TRANSITIONS ALONG WITH THE ANIMATIONS
 }
  
}
