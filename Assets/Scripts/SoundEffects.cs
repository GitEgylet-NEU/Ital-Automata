using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    AudioSource Blugy = new();
    AudioSource Brrrrr = new();
    AudioSource Boop = new();
    AudioSource Lotty = new();

    private void Start()
    {
        Blugy.clip = Resources.Load<AudioClip>("SFX/blugy");
        Brrrrr.clip = Resources.Load<AudioClip>("SFX/brrrrr");
        Boop.clip = Resources.Load<AudioClip>("SFX/boop");
        Lotty.clip = Resources.Load<AudioClip>("SFX/lotty");
        UIcontroller.instance.onCalculateButtonPressed.AddListener(() => PlaySound("boop"));
        UIcontroller.instance.onCalculateButtonPressed.AddListener(() => PlaySound("brrrrr"));
        //Dispenser.instance.onButtonStateChanged.AddListener((bool x) => PlaySound("blugy"));
        GyroscopeHandler.instance.onGyroButtonFlashed.AddListener(() => PlaySound("lotty"));
    }

    void PlaySound(string name)
    {
        switch (name)
        {
            case "blugy":
                Blugy.Play();
                break;
            case "brrrrr":
                Brrrrr.Play();
                break;
            case "boop":
                Boop.Play();
                break;
            case "lotty":
                Lotty.Play();
                break;
        }
    }
}
