using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script control the In-Game settings of the Game.

public class SettingsMenu : MonoBehaviour
{
    public void SetFullscreen(bool ActiveFullScreen)
    {
        //Changes from Windowed Mode to FullScreen
        Screen.fullScreen = ActiveFullScreen;
    }

    public void SetGameQuality(int qualityLevel)
    {
        //Changes Quality preset
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void SetGameVolume(float Vol)
    {
        //mainMixer.SetFloat("Volume", Vol); //To Be Added soon
    }
}


