using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvas : CanvasPresent
{
    private void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.Victory, ActivateCanvas);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.Victory, ActivateCanvas);
    }

}
