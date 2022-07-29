using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _timer;
    [SerializeField] TextMeshProUGUI _deathCounter;

    private float _currentTimer;
    private bool _isTimerActive;

    private void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.GameOver, UpdateDeathCounter);
        EventBus.Subscribe(GameplayEventType.Victory, UpdateLevelTime);

        EventBus.Subscribe(GameplayEventType.GameOver, DisableTimer);
        EventBus.Subscribe(GameplayEventType.Victory, DisableTimer);

    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.GameOver, UpdateDeathCounter);
        EventBus.Unsubscribe(GameplayEventType.Victory, UpdateLevelTime);

        EventBus.Unsubscribe(GameplayEventType.GameOver, DisableTimer);
        EventBus.Unsubscribe(GameplayEventType.Victory, DisableTimer);

    }

    private void UpdateDeathCounter(UnityEngine.Object sender, EventArgs args)
    {
        var oldData = SaveSystem.GetLevelData(GameManager.Instance.LevelId);
        LevelData newData = new LevelData(oldData.DeathCount + 1, oldData.BestTime, oldData.IsCompleted);
        SaveSystem.SaveLevelData(newData, GameManager.Instance.LevelId);
        _deathCounter.text = newData.DeathCount.ToString();
    }

    void Start()
    {
        _deathCounter.text = SaveSystem.GetLevelData(GameManager.Instance.LevelId).DeathCount.ToString();
        _currentTimer = 0f;
        _isTimerActive = true;
    }

    private void DisableTimer(UnityEngine.Object sender, EventArgs args)
    {
        _isTimerActive = false;
    }

    private void UpdateLevelTime(UnityEngine.Object sender, EventArgs args)
    {
        var oldData = SaveSystem.GetLevelData(GameManager.Instance.LevelId);
        float time = _currentTimer < oldData.BestTime || oldData.BestTime <= 0 ? _currentTimer : oldData.BestTime;
        LevelData newData = new LevelData(oldData.DeathCount, time, oldData.IsCompleted);
        SaveSystem.SaveLevelData(newData, GameManager.Instance.LevelId);
        _deathCounter.text = newData.DeathCount.ToString();
    }

    void Update()
    {
        if (_isTimerActive)
        {
            _currentTimer += Time.deltaTime;

            int minutes = (int)_currentTimer / 60;
            int seconds = (int)_currentTimer - minutes * 60;
            int milliseconds =(int)((_currentTimer - minutes * 60 - seconds) * 100);
            _timer.text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + milliseconds.ToString("D2");
        }
    }
}
