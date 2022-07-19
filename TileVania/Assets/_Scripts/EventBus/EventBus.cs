using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventBus
{
    private static readonly IDictionary<GameplayEventType, UnityEvent<UnityEngine.Object, EventArgs>> Events = new Dictionary<GameplayEventType, UnityEvent<UnityEngine.Object, EventArgs>>();

    public static void Subscribe(GameplayEventType eventType, UnityAction<UnityEngine.Object, EventArgs> listener)
    {
        UnityEvent<UnityEngine.Object, EventArgs> thisEvent;
        if(Events.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<UnityEngine.Object, EventArgs>();
            thisEvent.AddListener(listener);
            Events.Add(eventType, thisEvent);
        }
    }

    public static void Unsubscribe(GameplayEventType eventType, UnityAction<UnityEngine.Object, EventArgs> listener)
    {
        UnityEvent<UnityEngine.Object, EventArgs> thisEvent;
        if(Events.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void Publish(GameplayEventType eventType, UnityEngine.Object sender, EventArgs args)
    {
        UnityEvent<UnityEngine.Object, EventArgs> thisEvent;
        if(Events.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.Invoke(sender, args);
        }
    }
}
