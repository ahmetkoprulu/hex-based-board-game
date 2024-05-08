using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new();
    private static bool IsQuiting;

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null && !IsQuiting)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        Init();
        if (_instance == null) _instance = gameObject.GetComponent<T>();
        else if (_instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
            throw new System.Exception("An instance of this singleton already exists.");
        }
    }
    protected virtual void OnApplicationQuit() => IsQuiting = true;

    protected virtual void Init() { }
}
