using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    public readonly GridEventManager HexGridEventManager = new();
}

public class GridEventManager
{
    public EventChannel<List<HexCell>> PathFoundChannel = new();
}
