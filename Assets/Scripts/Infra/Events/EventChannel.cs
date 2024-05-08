using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventChannel<T> : Dictionary<string, GameEvent<T>> where T : class, new()
{
    public void Subscribe(string eventName, Action<Component, T> action)
    {
        if (!ContainsKey(eventName)) this[eventName] = new GameEvent<T>();

        this[eventName].AddListener(action.Invoke);
    }

    public void Unsubscribe(string eventName, Action<Component, T> action)
    {
        if (!ContainsKey(eventName)) return;

        this[eventName].RemoveListener(action.Invoke);
    }

    public void Publish(string eventName, Component sender, T data)
    {
        if (!ContainsKey(eventName)) return;

        this[eventName].Invoke(sender, data);
    }

    public void PublishAll(Component sender, T data)
    {
        foreach (var item in this) item.Value.Invoke(sender, data);
    }

    public void ClearAll()
    {
        foreach (var item in this) item.Value.RemoveAllListeners();

        Clear();
    }

    public void Clear(string eventName)
    {
        if (!ContainsKey(eventName)) return;

        this[eventName].RemoveAllListeners();
    }
}

public class GameEvent<T> : UnityEvent<Component, T> where T : class, new() { }