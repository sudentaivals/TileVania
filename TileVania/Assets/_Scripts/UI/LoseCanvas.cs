using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseCanvas : CanvasPresent
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe(GameplayEventType.GameOver, ActivateCanvas);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Unsubscribe(GameplayEventType.GameOver, ActivateCanvas);
    }


}
