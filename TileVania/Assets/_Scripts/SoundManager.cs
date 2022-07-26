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
            AddSoundSource();
        }
        SoundVolume = PlayerPrefsController.SfxVolume;
    }

    private AudioSource AddSoundSource()
    {
        var go = new GameObject();
        go.transform.parent = transform;
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        _audioSources.Add(audioSource);
        return audioSource;

    }

    private void PlaySound(UnityEngine.Object sender, EventArgs args)
    {
        var soundArgs = args as PlaySoundEventArgs;
        if (soundArgs.Clip == null) return;
        var availableSource = _audioSources.FirstOrDefault(a => !a.isPlaying);
        var pos = sender as Component;
        if (availableSource == null)
        {
            var newSource = AddSoundSource();
            newSource.transform.position = pos.transform.position;
            newSource.clip = soundArgs.Clip;
            newSource.volume = soundArgs.Volume * SoundVolume;
            newSource.Play();
        }
        else
        {
            availableSource.transform.position = pos.transform.position;
            availableSource.clip = soundArgs.Clip;
            availableSource.volume = soundArgs.Volume * SoundVolume;
            availableSource.Play();
        }

    }
}
