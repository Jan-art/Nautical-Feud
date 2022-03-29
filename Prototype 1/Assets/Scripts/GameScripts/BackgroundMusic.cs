using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
   private static BackgroundMusic backgroundMusic;
    public AudioSource bgMusic;

   void Awake()
   {
       if(backgroundMusic == null)
       {
           backgroundMusic = this;
           DontDestroyOnLoad(backgroundMusic);
       }
       else
       {
          Destroy(gameObject);
       }
        bgMusic.SetScheduledEndTime(AudioSettings.dspTime + (75));

    }

    void Update()
    {
        if (!bgMusic.isPlaying)
        {
            bgMusic.Play();
            bgMusic.SetScheduledEndTime(AudioSettings.dspTime + (75));
        }
    }
}

