using UnityEngine;

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
	public readonly string[] tracks = new string[]
	{
		"Local Forecast (Elevator)",
		"The Cannery",
		"Gaslamp Funworks",
		"Stringed Disco"
	};
	int currentTrack;
	public bool musicPlaying { get; private set; }
	public bool musicPaused { get; private set; }
	AudioSource Music;

	void Start()
	{
		Music = GetComponent<AudioSource>();
		Music.volume = .35f;
		Music.clip = Resources.Load<AudioClip>("Audio/elevatormusic");
		currentTrack = 0;
		Music.Play();
		musicPlaying = true;
		musicPaused = false;
	}

	public void ChangeMusic(string track)
	{
		musicPaused = false;
		Music.Stop();
		switch (track)
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
		currentTrack = System.Array.IndexOf(tracks, track);
	}

	public void ChangeMusic(int i)
	{
		string track = tracks[i];
		ChangeMusic(track);
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
		musicPaused = false;
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
		//if (!musicPlaying) return;
		int i = currentTrack + 1;
		if (i >= tracks.Length) i = 0;
		ChangeMusic(i);
		if (!musicPlaying) StartStop();
	}

	public string GetCurrentTrack()
	{
		if (!musicPlaying) return string.Empty;
		return tracks[currentTrack];
	}

	public void SetVolume(float volume)
	{
		Music.volume = volume;
	}
	public float GetVolume()
	{
		return Music.volume;
	}
}