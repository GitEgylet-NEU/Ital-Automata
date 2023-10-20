using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    private void Start()
    {
        UIcontroller.instance.onCalculateButtonPressed.AddListener(() => PlaySound("boop"));
        UIcontroller.instance.onCalculateButtonPressed.AddListener(() => PlaySound("brrrrr"));
        GyroscopeHandler.instance.onGyroButtonFlashed.AddListener(() => PlaySound("nope"));
    }

    public void PlaySound(string name)
    {
        AudioSource SFX = gameObject.AddComponent<AudioSource>();
        switch (name)
        {
            case "blugy":
                SFX.clip = Resources.Load<AudioClip>("SFX/blugy");
                break;
            case "brrrrr":
                SFX.clip = Resources.Load<AudioClip>("SFX/brrrrr");
                break;
            case "boop":
                SFX.clip = Resources.Load<AudioClip>("SFX/boop");
                break;
            case "lotty":
                SFX.clip = Resources.Load<AudioClip>("SFX/lotty");
                break;
            case "nope":
                SFX.clip = Resources.Load<AudioClip>("SFX/nope");
                break;
            case "nope2":
                SFX.clip = Resources.Load<AudioClip>("SFX/nope2");
                break;
        }
        SFX.Play();
        IEnumerator RemoveSFX()
        {
            yield return new WaitForSeconds(SFX.clip.length);
            Destroy(SFX);
        }
        StartCoroutine(RemoveSFX());
    }
}
