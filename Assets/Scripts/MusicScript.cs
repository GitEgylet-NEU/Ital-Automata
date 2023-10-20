using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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
    string[] tracks = new string[]
    {
        "Local Forecast (Elevator)",
        "The Cannery",
        "Gaslamp Funworks",
        "Stringed Disco"
    };
    int currentTrack;
    bool musicPlaying;
    bool musicPaused;
    AudioSource Music;
    //[SerializeField] TMPro.TMP_Dropdown Select;

    void Start()
    {
        Music = GetComponent<AudioSource>();
        Music.clip = Resources.Load<AudioClip>("Audio/elevatormusic");
        currentTrack = 0;
        Music.Play();
        musicPlaying = true;
        musicPaused = false;
    }

    public void ChangeMusic()
    {
        musicPaused = false;
        Music.Stop();
        string option = tracks.First();
        //var option = Select.options[Select.value].text;
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

	public void ChangeMusic(int i)
	{
		musicPaused = false;
		Music.Stop();
		switch (tracks[i])
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
        currentTrack = i;
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

    public void Next()
    {
        int i = currentTrack++;
        if (i >= tracks.Length) i = 0;
        ChangeMusic(i);
    }
}
