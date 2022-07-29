using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonInstance<GameManager>
{
    [SerializeField] AudioClip _victorySfx;
    [SerializeField] [Range(0f, 1f)] float _victorySfxVolume = 1f;
    [SerializeField] int _thisLevelId;
    public int LevelId => _thisLevelId;

    void Start()
    {
        TriggerGameState(GameState.Play);
    }

    public void TriggerGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Play:
                Time.timeScale = 1f;
                EventBus.Publish(GameplayEventType.Unpause, this, new System.EventArgs());
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                EventBus.Publish(GameplayEventType.Pause, this, new System.EventArgs());
                break;
            case GameState.Lose:
                EventBus.Publish(GameplayEventType.GameOver, this, null);
                break;
            case GameState.Victory:
                EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_victorySfxVolume, _victorySfx));
                SetLevelCompleted();
                EventBus.Publish(GameplayEventType.Victory, this, null);
                break;
            default:
                break;
        }
    }

    private void SetLevelCompleted()
    {
        var data = SaveSystem.GetLevelData(LevelId);
        var newData = new LevelData(data.DeathCount, data.BestTime, true);
        SaveSystem.SaveLevelData(newData, LevelId);
    }

    public void Win()
    {
        TriggerGameState(GameState.Victory);
    }
}

public enum GameState
{
    Play,
    Pause,
    Restart,
    Lose,
    Victory,
}
