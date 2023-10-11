using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{

    bool musicPlaying = false;
    bool musicPaused = false;
    AudioSource Music;

    void Start()
    {
        Music = GetComponent<AudioSource>();
        Music.Stop();
    }

    public void StartStop()
    {
        musicPlaying = !musicPlaying;
        if (musicPlaying)
        {
            Music.Play();
        }
        else
        {
            Music.Stop();
        }
    }

    public void Pause()
    {
        if (musicPlaying)
        {
            musicPaused = !musicPaused;
            if (musicPaused)
            {
                Music.Pause();
            }
            else
            {
                Music.UnPause();
            }
        }
    }
}
