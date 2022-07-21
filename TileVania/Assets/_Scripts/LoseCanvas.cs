using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseCanvas : MonoBehaviour
{
    [SerializeField] Image _darkScreen;
    [SerializeField] Transform _buttons;
    [SerializeField] Transform _message;
    [SerializeField] float _activationDelay;
    [SerializeField] float _startYOffset;
    [SerializeField] Button _firstSelectedButton;
    private float _currentDelayTimer;
    private bool _isActivating = false;
    private Vector3 _buttonsStartPos;
    private Vector3 _messageStartPos;
    private float _darkScreenStartAlpha;

    private void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.GameOver, ActivateCanvas);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.GameOver, ActivateCanvas);
    }

    private void Start()
    {
        _buttonsStartPos = _buttons.position;
        _messageStartPos = _message.position;
        _darkScreenStartAlpha = _darkScreen.color.a;
    }

    private void ActivateCanvas(UnityEngine.Object sender, EventArgs args)
    {
        _isActivating = true;
        _currentDelayTimer = 0;
        _buttons.gameObject.SetActive(true);
        _message.gameObject.SetActive(true);
        _darkScreen.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_isActivating)
        {
            _buttons.position = Vector3.Lerp(_buttonsStartPos + new Vector3(0, _startYOffset, 0), _buttonsStartPos, _currentDelayTimer / _activationDelay);
            _message.position = Vector3.Lerp(_messageStartPos + new Vector3(0, _startYOffset, 0), _messageStartPos, _currentDelayTimer / _activationDelay);
            _darkScreen.color = new Color(_darkScreen.color.r, _darkScreen.color.g, _darkScreen.color.b, Mathf.Lerp(0, _darkScreenStartAlpha, _currentDelayTimer / _activationDelay));
            _currentDelayTimer += Time.deltaTime;
            if (_currentDelayTimer >= _activationDelay)
            {
                _currentDelayTimer = 0;
                _buttons.position = _buttonsStartPos;
                _message.position = _messageStartPos;
                _darkScreen.color = new Color(_darkScreen.color.r, _darkScreen.color.g, _darkScreen.color.b, _darkScreenStartAlpha);
                _isActivating = false;
                foreach (Transform tr in _buttons)
                {
                    var button = tr.GetComponent<Button>();
                    if (button != null) button.interactable = true;
                }
                _firstSelectedButton.Select();
            }
        }
    }

    public void Restart()
    {
        SceneLoader.Instance.RestartScene();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
