using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : CanvasPresent
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe(GameplayEventType.Pause, ActivateCanvas);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Unsubscribe(GameplayEventType.Pause, ActivateCanvas);
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetButtonDown("Pause") && !_isAnyCanvasActive)
        {
            GameManager.Instance.TriggerGameState(GameState.Pause);
        }
    }
}
