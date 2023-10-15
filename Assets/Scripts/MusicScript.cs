using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicScript : MonoBehaviour
{

    bool musicPlaying = false;
    bool musicPaused = false;
    AudioSource Music;
    TMPro.TMP_Dropdown Select;

    void Start()
    {
        Music = GetComponent<AudioSource>();
        Select = GetComponent<TMPro.TMP_Dropdown>();
        Music.clip = Resources.Load<AudioClip>("Audio/elevatormusic");
        Music.Stop();
    }

    public void ChangeMusic()
    {
        musicPlaying = false;
        musicPaused = false;
        Music.Stop();
        var option = Select.options[Select.value].text; //NullReferenceException: Object reference not set to an instance of an object
        if (option == "Local Forecast (Elevator)")
        {
            Music.clip = Resources.Load<AudioClip>("Audio/elevatormusic");
        }
        else if (option == "The Cannery")
        {
            Music.clip = Resources.Load<AudioClip>("Audio/thecannery");
        }
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
