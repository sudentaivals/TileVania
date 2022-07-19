using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEventArgs : EventArgs
{
    public float Volume { get; }

    public AudioClip Clip { get; }

    public PlaySoundEventArgs(float volume, AudioClip clip)
    {
        Volume = volume;
        Clip = clip;
    }
}