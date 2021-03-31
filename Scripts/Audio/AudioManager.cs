using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;	
	public AudioMixerGroup mixerGroup;
	public Sound[] sounds;
	private static bool created = false;
	void Awake()
	{
		if (instance != null)
		{
			instance = this;
		}
		else
		{
			DontDestroyOnLoad(this.gameObject);
		}
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

	public void Play(string sound)
	{	
			
		if (sound == "Normal_BG")
		{
			string[] songList = new string[] { "Stage_Normal", "Stage_Normal1", "Stage_Normal2", "Stage_Normal3", "Stage_Normal4" };
        	int songPicked = UnityEngine.Random.Range(0,4); 
        	sound = songList[songPicked];
			Sound s = Array.Find(sounds, item => item.name == sound);
			s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
			s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
			s.source.Play();
		}
		else if (sound == "Boss_BG")
		{
			string[] songList = new string[] { "Stage_Boss", "Stage_Boss1", "Stage_Boss2", "Stage_Boss3"};
        	int songPicked = UnityEngine.Random.Range(0,4); 
        	sound = songList[songPicked];
			Sound s = Array.Find(sounds, item => item.name == sound);
			s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
			s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
			s.source.Play();
		}
		else
		{
			Sound s = Array.Find(sounds, item => item.name == sound);
			//Displays to console if sound file name not found		
			/*if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}	*/
			s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
			s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
			s.source.Play();
		}
	}

	public void PlayOnce(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
		s.source.PlayOneShot(s.source.clip);
	}
	public void Stop(string sound)
	{

		if (sound == "Stage_BG")
		{
			string[] songList = new string[] { "Stage_Normal", "Stage_Normal1", "Stage_Normal2", "Stage_Normal3", "Stage_Normal4",
												"Stage_Boss", "Stage_Boss1", "Stage_Boss2", "Stage_Boss3" };
			for (int i = 0; i < 9; i++)
			{
				Sound s = Array.Find(sounds, item => item.name == songList[i]);						
				s.source.volume = 0;
				s.source.pitch = 0;
				s.source.Stop();
			}
		}
		else
		{
			Sound s = Array.Find(sounds, item => item.name == sound);	
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}
			s.source.volume = 0;
			s.source.pitch = 0;
			s.source.Stop();
		}
	}

}
