using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicScript : MonoBehaviour
{

    bool musicPlaying = false;
    bool musicPaused = false;
    AudioSource Music;
    [SerializeField] TMPro.TMP_Dropdown Select; //a [SerializeField] tag-gel megjelenik a mező a Unity inspectorban, így bele lehet húzni dolgokat

    void Start()
    {
        Music = GetComponent<AudioSource>();
        //Select = GetComponent<TMPro.TMP_Dropdown>(); //a MusicScript GameObject-jén nincsen Dropdown komponens, tehát külön meg kell mondani a scriptnek, hogy melyikre gondolsz (lásd feljebb)
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
