using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvas : CanvasPresent
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe(GameplayEventType.Victory, ActivateCanvas);
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Unsubscribe(GameplayEventType.Victory, ActivateCanvas);
    }

}
