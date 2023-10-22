using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPlayerSeen : MonoBehaviour
{
    [SerializeField] private UnityEvent OnSeen;
    public void IsSeen(Component sender, object data)
    {
        if(data is not GameObject player) return;
        if(player != gameObject)  return;
        Debug.Log("Player seen");
        OnSeen?.Invoke();
    }
}
