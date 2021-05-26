using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : Singleton<SoundController>
{
    private AudioSource backgroundAudio;
    private AudioSource onceAudio;
    private List<AudioClip> audioClips = new List<AudioClip>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        backgroundAudio = gameObject.AddComponent<AudioSource>();
        backgroundAudio.playOnAwake = true;
        backgroundAudio.loop = true;

        onceAudio = gameObject.AddComponent<AudioSource>();
        onceAudio.playOnAwake = true;

        for (int i = 0; i < Enum.GetNames(typeof(SoundType)).Length; i++)
        {
            SoundData soundData = SoundResources.Instance.SoundDatas.Find(item => item.SoundType == (SoundType)i);
            audioClips.Add(soundData.Clip);
        }
    }

    public void PlayOnce(SoundType soundType)
    {
        if (!Data.SoundState) return;

        AudioClip clip = audioClips[(int)soundType];

        onceAudio.PlayOneShot(clip);
    }

    public void PlayBackground(SoundType soundType)
    {
        if (!Data.MusicState) return;

        AudioClip clip = audioClips[(int)soundType];

        if (clip && backgroundAudio.clip != clip)
        {
            backgroundAudio.clip = clip;
        }
        backgroundAudio.Play();
    }

    public void PauseBackground()
    {
        if (backgroundAudio)
        {
            backgroundAudio.Pause();
        }
    }

    public AudioSource PlayLoop(SoundType soundType)
    {
        if (!Data.SoundState) return null;

        AudioClip clip = audioClips[(int)soundType];

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.loop = true;
        audioSource.Play();

        return audioSource;
    }
}
