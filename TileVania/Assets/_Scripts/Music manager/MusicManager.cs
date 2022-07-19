using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicManager : SingletonInstance<MusicManager>
{
    [SerializeField] List<BaseMusicState> _musicManagerStates;
    private BaseMusicState _currentState;
    private AudioSource _source;
    private int _currentTrackIndex = 0;
    public float MusicVolume
    {
        get
        {
            return _musicVolume;
        }
        set
        {
            _musicVolume = value;
            _source.volume = _musicVolume;
        }
    }

    private float _musicVolume;


    public override void Awake()
    {
        base.Awake();
        _source = GetComponent<AudioSource>();
        MusicVolume = PlayerPrefsController.MusicVolume;
    }

    private void Start()
    {
        Transition(_musicManagerStates[0].StateName);
    }

    private void SelectNextTrack()
    {
        if(_currentTrackIndex+1 == _currentState.Clips.Count)
        {
            _currentTrackIndex = 0;
        }
        else
        {
            _currentTrackIndex++;
        }
        _source.clip = _currentState.Clips[_currentTrackIndex];
    }

    public void Transition(string newState)
    {
        try
        {
            _currentState = _musicManagerStates.First(a => a.StateName == newState);
        }
        catch
        {
            _currentState = _musicManagerStates[0];
        }
        _source.clip = _currentState.Clips[0];
    }

    private void Update()
    {
        if (!_source.isPlaying)
        {
            SelectNextTrack();
            _source.Play();
        }
    }

}
