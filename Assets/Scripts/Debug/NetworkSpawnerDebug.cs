using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawnerDebug : MonoBehaviour
{
    public GameObject Prefab;
    public void Spawn()
    {
        var obj = Instantiate(Prefab);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.GetComponent<NetworkManager>().StartHost();
    }
}
