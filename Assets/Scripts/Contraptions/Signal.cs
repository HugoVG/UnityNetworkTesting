using Unity.Netcode;
using UnityEngine;

public abstract class Signal<T> : NetworkBehaviour where T : unmanaged
{
    public abstract NetworkVariable<T> Value { get; }
}