using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonInstance<GameManager>
{
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
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
            case GameState.Lose:
                EventBus.Publish(GameplayEventType.GameOver, this, null);
                break;
            case GameState.Victory:

                break;
            default:
                break;
        }
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
