using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
	float volume = .75f;
	float ido;

	List<AudioSource> sources;

	private void Start()
	{
		UIcontroller.instance.onTimeChanged.AddListener((float t) => ido = t);

		UIcontroller.instance.onCalculateButtonPressed.AddListener(() => PlaySound("boop"));
		UIcontroller.instance.onCalculateButtonPressed.AddListener(() => PlayBrr(ido));
		GyroscopeHandler.instance.onGyroButtonFlashed.AddListener(() => PlaySound("nope"));

		sources = new List<AudioSource>();
	}

	public void PlaySound(string name)
	{
		AudioSource SFX = gameObject.AddComponent<AudioSource>();

		try
		{
			SFX.clip = LoadClip(name);
		}
		catch (System.Exception)
		{
			Debug.LogWarning($"Can't load SFX \"{name}\"");
			Destroy(SFX);
			return;
		}

		SFX.volume = volume;
		sources.Add(SFX);
		SFX.Play();

		StartCoroutine(RemoveSFX());
		IEnumerator RemoveSFX()
		{
			yield return new WaitForSeconds(SFX.clip.length);
			sources.Remove(SFX);
			Destroy(SFX);
		}
	}

	AudioClip LoadClip(string name)
	{
		AudioClip clip = Resources.Load<AudioClip>($"SFX/{name}");
		if (clip == null)
		{
			throw new System.IO.FileNotFoundException($"Couldn't find an AudioClip at Resources/SFX/{name}");
		}
		return clip;
	}

	void PlayBrr(float time)
	{
		AudioSource brr = gameObject.AddComponent<AudioSource>();
		brr.volume = volume;
		sources.Add(brr);

		StartCoroutine(DoBrr());
		IEnumerator DoBrr()
		{
			brr.clip = LoadClip("brr-loop");
			brr.loop = true;
			brr.Play();

			yield return new WaitForSeconds(time);
			brr.Stop();
			brr.loop = false;
			brr.clip = LoadClip("brr-stop");
			brr.Play();

			yield return new WaitWhile(() => brr.isPlaying);
			sources.Remove(brr);
			Destroy(brr);
		}
	}

	public float GetVolume() => volume;
	public void SetVolume(float volume)
	{
		this.volume = volume;
		foreach (var src in sources)
		{
			src.volume = this.volume;
		}
	}
}