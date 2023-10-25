using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using Unity.Netcode;
using UnityEngine;

public class GeneralCheatCodes : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject NetworkPrefav;
    // Start is called before the first frame update
    void Start()
    {
        DebugLogConsole.AddCommand<Vector3>("SpawnPlayer", "Spawns a Player character", SpawnPlayer);
        DebugLogConsole.AddCommand("SpawnNetwork", "Spawns a network object", SpawnNetwork);
    }
    // Spawn player
    public void SpawnPlayer(Vector3 place = default)
    {
        // Find GameObject with NetworkManager
        var obj = FindObjectOfType<NetworkManager>();
        if (obj is null)
        {
            SpawnNetwork();
        }
        
        place = place == default ? new Vector3(0, 1, 0) : place;
        var player = Instantiate(PlayerPrefab, place, Quaternion.identity);
        player.GetComponent<CameraController>().cameraHolder.SetActive(true);
    }

    public void SpawnNetwork()
    {
        var obj = Instantiate(NetworkPrefav);
        obj.GetComponent<NetworkManager>().StartHost();        
    }
}
