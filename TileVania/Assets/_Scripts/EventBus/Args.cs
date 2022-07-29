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

public class SaveLevelDataEventArgs : EventArgs
{
    public float Time { get; }

    public int Deaths { get; }

    public bool IsCompleted { get; }

    public SaveLevelDataEventArgs(float time, int deaths, bool isCompleted)
    {
        Time = time;
        Deaths = deaths;
        IsCompleted = isCompleted;
    }
}