using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[HideScriptField]
public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject foxPrefab;
    [SerializeField] private GameObject badgerPrefab;
    
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnOnLoadEventCompleted;
        base.OnNetworkDespawn();
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Makes sure that the player is spawned on the host and clients
    /// </summary>
    /// <param name="scenename">Scene name that gets loaded</param>
    /// <param name="loadscenemode">What type of mode</param>
    /// <param name="clientscompleted">Which of the clients were brought over to the new scene</param>
    /// <param name="clientstimedout">Which clients got lost in transport</param>
    private void SceneManagerOnOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (!IsHost || scenename != "SampleScene") return;
        foreach (var id in clientscompleted)
        {
            var data = SessionManager<SessionData>.Instance.GetPlayerData(id);
            Debug.Log($"{id}, {data.PlayerName}, {data.PlayerTeam}");
            
            
            //Check if the client is the host
            bool ishostingplayer = id == NetworkManager.Singleton.LocalClientId;
            GameObject newPlayer;
            if (data.PlayerTeam == Team.Fox)
            {
                newPlayer = (GameObject)Instantiate(foxPrefab);
            }
            else
            {
                newPlayer = (GameObject)Instantiate(badgerPrefab);
            }
            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
        }
    }
}
