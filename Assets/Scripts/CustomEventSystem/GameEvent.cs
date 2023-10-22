using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    public List<GameEventListener> Listeners = new ();
    public void RaiseEvent(Component sender, object data)
    {
        foreach (var listener in Listeners)
        {
            listener.OnEventRaised(sender, data);
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if(!Listeners.Contains(listener)) Listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if(Listeners.Contains(listener)) Listeners.Remove(listener);
    }
}
