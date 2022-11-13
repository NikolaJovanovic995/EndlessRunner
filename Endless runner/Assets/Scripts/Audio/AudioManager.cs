using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Play(string soundName)
    {
        Sound sound = FindSound(soundName);
        if (sound == null)
            return;

        if(sound.MixerGroup == MixerGroup.SFX)
        {
            sfxSource.volume = sound.Volume;
            sfxSource.PlayOneShot(sound.AudioClip);
        }
        else if(sound.MixerGroup == MixerGroup.MUSIC)
        {
            musicSource.Stop();
            musicSource.clip = sound.AudioClip;
            musicSource.volume = sound.Volume;
            musicSource.Play();
        }
    }

    public void PauseMusic()
    {
        if(musicSource.isPlaying)
            musicSource.Pause();    
    }

    public void UnpauseMusic()
    {
        if (musicSource.isPlaying == false)
            musicSource.UnPause();
    }

    private Sound FindSound(string soundName)
    {
        Sound lSound = Array.Find(sounds, sound => sound.Name == soundName);
        if (lSound == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
        return lSound;
    }
}
