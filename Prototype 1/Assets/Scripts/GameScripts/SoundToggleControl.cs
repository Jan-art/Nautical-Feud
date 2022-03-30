using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This Script controls the Sound Toggle Function/States

public class SoundToggleControl : MonoBehaviour
{

    [SerializeField] Image soundOn;
    [SerializeField] Image soundOff;

    private bool muted = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("muted"))
        {
            PlayerPrefs.SetInt("muted", 0);
            LoadAudioData();
        }
        else
        {
          LoadAudioData();
        }

        UpdateBtnImage();
        AudioListener.pause = muted;
    }

    public void OnBtnPress()
    {
      if(muted ==  false)
      {
         muted = true;
         AudioListener.pause = true;
      }
      
      else
      {
        muted = false;
        AudioListener.pause = false;
      }

      SaveAudioData();
      UpdateBtnImage();
    }

    //Update Icon
    private void UpdateBtnImage()
    {
      if(muted == false)
      {
        soundOn.enabled = true;
        soundOff.enabled = false;
      }
      else
      {
        soundOn.enabled = false;
        soundOff.enabled = true;
      }
     
    }

    //Load Saved Values
    private void LoadAudioData()
    {
      muted = PlayerPrefs.GetInt("muted") == 1;
    }


    //Save Values
    private void SaveAudioData()
    {
       PlayerPrefs.SetInt("muted", muted ? 1 : 0);
    }

   
}
