using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script control the In-Game settings of the Game.

public class SettingsMenu : MonoBehaviour
{

    public GameObject Check;

    void start()
    {
        Check.SetActive(false);
    }

    public void SetFullscreen(bool ActiveFullScreen)
    {
        //Changes from Windowed Mode to FullScreen
        Screen.fullScreen = ActiveFullScreen;
        Check.SetActive(true);
        Debug.Log("Set to FullScreen");

    }

    public void SetGameQuality(int qualityLevel)
    {
        //Changes Quality preset
        QualitySettings.SetQualityLevel(qualityLevel);
        Debug.Log("QualityPreset Changed");
    }

    public void SetGameVolume(float Vol)
    {
        //mainMixer.SetFloat("Volume", Vol); //To Be Added soon
    }
}


