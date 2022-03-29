using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

//This Script controls the In-Game settings of the Game.

public class SettingsMenu : MonoBehaviour
{
    
    //public AudioMixer mainMixer;
    public Toggle FullScreenToggle;

    //Res Types
    public List<ResItem> resolutionsAvailable = new List<ResItem>();
    
    private int chosenRes;

    public TMP_Text resLabel;

    [SerializeField] 
    Slider volSlider;

    void start()
    {
        //FullScreenToggle.isOn = Screen.fullScreen; //KEEP DISABLED

        bool foundRes = false;
        for(int i = 0; i < resolutionsAvailable.Count; i++)
        {
            if(Screen.width == resolutionsAvailable[i].horizontal && Screen.height == resolutionsAvailable[i].vertical)
            {
                foundRes = true;
                chosenRes = i;
                UpdateResLabel();
            }
        }
        
        //Adapt to Monitor Res
    
        if(!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
             newRes.vertical = Screen.height;

             resolutionsAvailable.Add(newRes);
             chosenRes = resolutionsAvailable.Count - 1;

             UpdateResLabel();
        }

        if(!PlayerPrefs.HasKey("gameVolume"))
        {
            PlayerPrefs.SetFloat("gameVolume", 1);
            LoadAudioSettings();
        }
        else
        {
          LoadAudioSettings();
        }
    }

//========================================================================
   
    public void ResLeft()
    {
        chosenRes --;
       if(chosenRes < 0)
       {
           chosenRes = 0;
       }

       UpdateResLabel();
       ChangeRes();
    }

    public void ResRight()
    {
       chosenRes++;
       if(chosenRes > resolutionsAvailable.Count - 1)
       {
           chosenRes = resolutionsAvailable.Count - 1;
       }

    
       UpdateResLabel();
       ChangeRes();
    }

//========================================================================

    public void UpdateResLabel(){
        resLabel.text = resolutionsAvailable[chosenRes].horizontal.ToString() + " x " + resolutionsAvailable[chosenRes].vertical.ToString();
    }
    
    public void ChangeRes() {
          Screen.SetResolution(resolutionsAvailable[chosenRes].horizontal, resolutionsAvailable[chosenRes].vertical, FullScreenToggle.isOn);
    }

//========================================================================

    public void SetFullscreen()
    {
        //Changes from Windowed Mode to FullScreen
        if(FullScreenToggle.isOn)
        {
            Screen.fullScreen = true;
            Debug.Log("Set to FullScreen");
        }
        else
        {
             Screen.fullScreen = false;
             Debug.Log("Set to Windowed");
        }
        
    }
    
//========================================================================  

    public void SetGameQuality(int qualityLevel)
    {
        //Changes Quality preset
        QualitySettings.SetQualityLevel(qualityLevel);
        Debug.Log("QualityPreset Changed");
    }

//========================================================================

    //Change Master Volume
    public void ChangeGameVolume()
    {
        AudioListener.volume = volSlider.value;
    }

    //Store Volume Settings

    private void LoadAudioSettings()
    {
       volSlider.value = PlayerPrefs.GetFloat("gameVolume");
    }

    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("gameVolume", volSlider.value);
    }


//========================================================================

    // Width + Height Inputs for ScreenRes
     [System.Serializable]
    public class ResItem
    {
        public int horizontal, vertical;
    }
}