using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
"Local Forecast - Elevator" Kevin MacLeod (incompetech.com)
"The Cannery" Kevin MacLeod (incompetech.com)
"Gaslamp Funworks" Kevin MacLeod (incompetech.com)
"Stringed Disco" Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 4.0 License
http://creativecommons.org/licenses/by/4.0/
*/

public class MusicScript : MonoBehaviour
{

    bool musicPlaying;
    bool musicPaused;
    AudioSource Music;
    [SerializeField] TMPro.TMP_Dropdown Select;

    void Start()
    {
        Music = GetComponent<AudioSource>();
        Music.clip = Resources.Load<AudioClip>("Audio/elevatormusic");
        Music.Play();
        musicPlaying = true;
        musicPaused = false;
    }

    public void ChangeMusic()
    {
        musicPaused = false;
        Music.Stop();
        var option = Select.options[Select.value].text;
        switch (option)
        {
            case "Local Forecast (Elevator)":
                Music.clip = Resources.Load<AudioClip>("Audio/elevatormusic");
                break;
            case "The Cannery":
                Music.clip = Resources.Load<AudioClip>("Audio/thecannery");
                break;
            case "Gaslamp Funworks":
                Music.clip = Resources.Load<AudioClip>("Audio/gaslampfunworks");
                break;
            case "Stringed Disco":
                Music.clip = Resources.Load<AudioClip>("Audio/stringeddisco");
                break;
        }
        if (musicPlaying)
        {
            Music.Play();
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
