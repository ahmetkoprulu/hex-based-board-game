using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent response;

    public void OnEnable()
    {
        gameEvent.Register(this);
    }

    public void OnDisable()
    {
        gameEvent.Unregister(this);
    }

    public void OnEventRaised()
    {
        response.Invoke();
    }
}