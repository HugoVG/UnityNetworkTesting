using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


/// <summary>
/// GameObject that in itself does nothing
/// but can be used to hook into the Netcode spawn and despawn events.
/// on scripts that do not have <see cref="NetworkBehaviour"/>
/// </summary>
public class NetcodeHooks : NetworkBehaviour
{
    public event Action OnNetworkSpawnHook;

    public event Action OnNetworkDespawnHook;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        OnNetworkSpawnHook?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        OnNetworkDespawnHook?.Invoke();
    }
}
