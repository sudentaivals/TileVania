using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseCanvas : CanvasPresent
{
    private void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.GameOver, ActivateCanvas);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.GameOver, ActivateCanvas);
    }


}
