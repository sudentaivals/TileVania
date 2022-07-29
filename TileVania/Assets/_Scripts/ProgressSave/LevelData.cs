using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int DeathCount { get; private set; } = 0;
    public float BestTime { get; private set; } = -1;

    public bool IsCompleted { get; private set; } = false;

    public LevelData(int deaths, float bestTime, bool isCompleted)
    {
        IsCompleted = isCompleted;
        DeathCount = deaths;
        BestTime = bestTime;
    }

}
