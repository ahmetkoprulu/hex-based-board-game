using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptables/GameEvent")]
public class GameEvent : ScriptableObject
{
    public List<GameEventListener> listeners = new();

    public void Register(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void Unregister(GameEventListener listener)
    {
        listeners.Remove(listener);
    }

    public void Trigger(Component sender, object data)
    {
        listeners.ForEach(listener => listener.OnEventRaised(sender, data));
    }
}