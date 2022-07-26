using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OneShotSwitch : InteractableObject
{
    [SerializeField] Sprite _pressedSprite;
    [SerializeField] Sprite _unpressedSprite;
    private SwitchState _currentState;
    private SpriteRenderer _sr;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.sprite = _unpressedSprite;
        _currentState = SwitchState.Unpressed;
    }
    public void ActionOnSwitch()
    {
        Interact();
        _isActive = false;
        _sr.sprite = _pressedSprite;
        _currentState = SwitchState.Pressed;

    }

    protected override void Update()
    {
        base.Update();
        if(_currentState == SwitchState.Unpressed)
        {
            if (IsPlayerInRange && Input.GetButtonDown("Interact"))
            {
                ActionOnSwitch();
            }
        }
    }

}

public enum SwitchState
{
    Pressed,
    Unpressed
}


