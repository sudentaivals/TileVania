using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMusicState : ScriptableObject
{
    [SerializeField] List<AudioClip> _stateClips;

    [SerializeField] bool _isLooping;

    public List<AudioClip> Clips => _stateClips;
    public abstract string StateName { get; }
}
