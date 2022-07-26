using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CanvasPresent : MonoBehaviour
{
    [SerializeField] List<Transform> _lerpingPositions;
    [SerializeField] List<Image> _lerpingAlphas;
    [SerializeField] float _activationDelay;
    [SerializeField] Vector3 _lerpOffset;
    [SerializeField] Button _firstSelectedButton;
    private float _currentDelayTimer;
    private bool _isActivating = false;
    private List<float> _startAlphaList;
    private List<Vector3> _startPositions;

    protected static bool _isAnyCanvasActive = false;

    protected virtual void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.LevelLoading, DeactivateCanvas);
        EventBus.Subscribe(GameplayEventType.Unpause, DeactivateCanvas);

    }

    protected virtual void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.LevelLoading, DeactivateCanvas);
        EventBus.Unsubscribe(GameplayEventType.Unpause, DeactivateCanvas);
    }

    private void Start()
    {

        _startPositions = new List<Vector3>();
        foreach (Transform tr in _lerpingPositions)
        {
            _startPositions.Add(tr.position);
        }

        _startAlphaList = new List<float>();
        foreach (var image in _lerpingAlphas)
        {
            _startAlphaList.Add(image.color.a);
        }
    }

    protected void DeactivateCanvas(UnityEngine.Object sender, EventArgs args)
    {
        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(false);
        }
        _isAnyCanvasActive = false;
    }

    protected void ActivateCanvas(UnityEngine.Object sender, EventArgs args)
    {
        if (_isAnyCanvasActive)
        {
            return;
        }
        else
        {
            _isAnyCanvasActive = true;
        }
        _isActivating = true;
        _currentDelayTimer = 0;

        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(true);
        }
    }

    private void ActivateButtons()
    {
        foreach (Transform tr in _lerpingPositions)
        {
            var buttons = tr.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                if (button != null) button.interactable = true;
            }
        }

        foreach (Image tr in _lerpingAlphas)
        {
            var buttons = tr.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                if (button != null) button.interactable = true;
            }
        }
    }


    protected virtual void Update()
    {
        if (_isActivating)
        {
            for (int i = 0; i < _lerpingPositions.Count; i++)
            {
                _lerpingPositions[i].position = Vector3.Lerp(_startPositions[i] + _lerpOffset, _startPositions[i], _currentDelayTimer / _activationDelay);
            }
            for (int i = 0; i < _lerpingAlphas.Count; i++)
            {
                _lerpingAlphas[i].color = new Color(_lerpingAlphas[i].color.r, _lerpingAlphas[i].color.g, _lerpingAlphas[i].color.b, Mathf.Lerp(0, _startAlphaList[i], _currentDelayTimer / _activationDelay));
            }
            _currentDelayTimer += Time.unscaledDeltaTime;
            if (_currentDelayTimer >= _activationDelay)
            {
                _currentDelayTimer = 0;
                for (int i = 0; i < _lerpingPositions.Count; i++)
                {
                    _lerpingPositions[i].position = _startPositions[i];
                }

                for (int i = 0; i < _lerpingAlphas.Count; i++)
                {
                    _lerpingAlphas[i].color = new Color(_lerpingAlphas[i].color.r, _lerpingAlphas[i].color.g, _lerpingAlphas[i].color.b, _startAlphaList[i]);
                }

                _isActivating = false;
                ActivateButtons();
                _firstSelectedButton.Select();
            }
        }
    }
}
