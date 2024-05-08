using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public CustomGameEvent response;

    public void OnEnable()
    {
        gameEvent.Register(this);
    }

    public void OnDisable()
    {
        gameEvent.Unregister(this);
    }

    public void OnEventRaised(Component sender, object data)
    {
        response.Invoke(sender, data);
    }
}