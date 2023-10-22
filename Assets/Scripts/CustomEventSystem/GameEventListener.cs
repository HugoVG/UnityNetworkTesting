using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object>{}

[System.Serializable]
public class GenericCustomGameEvent<T> : UnityEvent<Component, T>{}

public class GameEventListener : MonoBehaviour
{
    public GameEvent GameEvent;
    public CustomGameEvent response;
    
    private void OnEnable()
    {
        GameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        GameEvent.UnregisterListener(this);
    }

    public void OnEventRaised(Component sender, object obj)
    {
        response.Invoke(sender, obj);
    }
}