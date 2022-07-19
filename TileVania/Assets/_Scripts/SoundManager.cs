using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : SingletonInstance<SoundManager>
{
    private List<AudioSource> _audioSources;
    public float SoundVolume
    {
        get
        {
            return _soundMasterVolume;
        }
        set
        {
            _soundMasterVolume = value;
            foreach (var source in _audioSources)
            {
                source.volume = _soundMasterVolume;
            }
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.PlaySound, PlaySound);
    }


    private void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.PlaySound, PlaySound);
    }

    private float _soundMasterVolume;

    public override void Awake()
    {
        base.Awake();
        _audioSources = new List<AudioSource>();
        for (int i = 0; i < 5; i++)
        {
            AddNewSource();
        }
        SoundVolume = PlayerPrefsController.SfxVolume;
    }

    private void PlaySound(UnityEngine.Object sender, EventArgs args)
    {
        var soundArgs = args as PlaySoundEventArgs;
        if (soundArgs.Clip == null) return;
        var availableSources = _audioSources.FirstOrDefault(a => !a.isPlaying);
        if (availableSources == null)
        {
            var newSource = AddNewSource();
            newSource.clip = soundArgs.Clip;
            newSource.volume = soundArgs.Volume * SoundVolume;
            newSource.Play();
        }
        else
        {
            availableSources.clip = soundArgs.Clip;
            availableSources.volume = soundArgs.Volume * SoundVolume;
            availableSources.Play();
        }

    }

    private AudioSource AddNewSource()
    {
        var newAudioSource = gameObject.AddComponent<AudioSource>();
        newAudioSource.playOnAwake = false;
        newAudioSource.volume = SoundVolume;
        _audioSources.Add(newAudioSource);
        return newAudioSource;
    }

}
