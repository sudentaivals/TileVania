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

    const string DEATH_KEY = "DEATHS";
    private string SceneName => SceneManager.GetActiveScene().name;

    private void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.GameOver, UpdateDeathCounter);

        EventBus.Subscribe(GameplayEventType.GameOver, DisableTimer);
        EventBus.Subscribe(GameplayEventType.Victory, DisableTimer);

    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.GameOver, UpdateDeathCounter);

        EventBus.Unsubscribe(GameplayEventType.GameOver, DisableTimer);
        EventBus.Unsubscribe(GameplayEventType.Victory, DisableTimer);

    }

    private void UpdateDeathCounter(UnityEngine.Object sender, EventArgs args)
    {
        PlayerPrefsController.IncreaseDeathCounter(SceneName + DEATH_KEY);
        _deathCounter.text = PlayerPrefsController.GetDeathCounter(SceneName + DEATH_KEY).ToString();
    }

    void Start()
    {
        _deathCounter.text = PlayerPrefsController.GetDeathCounter(SceneName + DEATH_KEY).ToString();
        _currentTimer = 0f;
        _isTimerActive = true;
    }

    private void DisableTimer(UnityEngine.Object sender, EventArgs args)
    {
        _isTimerActive = false;
    }

    void Update()
    {
        if (_isTimerActive)
        {
            _currentTimer += Time.deltaTime;

            int minutes = (int)_currentTimer / 60;
            int seconds = (int)_currentTimer - minutes * 60;
            int milliseconds =(int)((_currentTimer - minutes * 60 - seconds) * 1000);
            _timer.text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + milliseconds.ToString("D3");
        }
    }
}
