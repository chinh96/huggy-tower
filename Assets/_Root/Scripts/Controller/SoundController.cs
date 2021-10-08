using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : Singleton<SoundController>
{
    [SerializeField] private AudioSource backgroundAudio;
    [SerializeField] private AudioSource onceAudio;
    private List<AudioClip> audioClips = new List<AudioClip>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        backgroundAudio.loop = true;

        for (int i = 0; i < Enum.GetNames(typeof(SoundType)).Length; i++)
        {
            SoundData soundData = ResourcesController.Sound.SoundDatas.Find(item => item.SoundType == (SoundType)i);
            audioClips.Add(soundData.Clip);
        }
    }

    public void PlayOnce(SoundType soundType)
    {
        AudioClip clip = audioClips[(int)soundType];

        if (!clip || !Data.SoundState) return;

        onceAudio.PlayOneShot(clip);
    }

    public void PlayBackground(SoundType soundType)
    {
        AudioClip clip = audioClips[(int)soundType];

        if (!clip || !Data.MusicState) return;

        backgroundAudio.clip = clip;
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
        AudioClip clip = audioClips[(int)soundType];

        if (!clip || !Data.SoundState) return null;

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = true;
        audioSource.loop = true;
        audioSource.Play();

        return audioSource;
    }
}
