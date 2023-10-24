using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Gameobject interface for <see cref="SessionManager{T}"/>
/// and makes sure that network callbacks are set up correctly
/// </summary>
public class SessionDataManager : NetworkBehaviour
{
    //Buffer for player names, in case the player RPCs before the server rpcs
    private Dictionary<ulong, string> _playerNameBuffer = new Dictionary<ulong, string>();
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject); // This object will persist between scenes
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong client)
    {
        if(IsClient) return; // Only run on the server
        SessionManager<SessionData>.Instance.DisconnectClient(client);
    }

    private void OnClientConnected(ulong client)
    {
        Debug.Log($"has connected with id {client}");
        if (IsServer)
        {
            Debug.Log("Server");
            // Check if the player is in the buffer
            var hasPlayer = _playerNameBuffer.TryGetValue(client, out string name);
            SessionManager<SessionData>.Instance.SetupConnectingPlayerSessionData(client, hasPlayer ? name : "UnKnown" , new SessionData()
            {
                PlayerName = hasPlayer ? name : "UnKnown",
                PlayerNumber = NetworkManager.Singleton.ConnectedClients.Count,
            });
            Debug.Log($"{(hasPlayer ? name : "UnKnown")} has connected with id {client}");
            if (hasPlayer)
            {
                _playerNameBuffer.Remove(client);
            }

            if (IsHost)
            {
                AnnounceName();
            }
        }
        else
        {

            Debug.Log("Client");
            AnnounceName();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AnnounceNameServerRpc(string name, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Announcing name");
        // Check if the player is in SessionManager
        if (SessionManager<SessionData>.Instance.GetPlayerData(serverRpcParams.Receive.SenderClientId) == null)
        {
            // put the name in the buffer
            _playerNameBuffer.Add(serverRpcParams.Receive.SenderClientId, name);
        }
        else
        {
            SetPlayerName(serverRpcParams.Receive.SenderClientId, name);
        }
    }
    
    public void AnnounceName()
    {
        Debug.Log("Announcing name root");
        string name = PlayerPrefs.GetString("PlayerName", $"Player {Random.Range(0, 1000)}");
        AnnounceNameServerRpc(name);
    }
    
    private void SetPlayerName(ulong clientId, string name)
    {
        Debug.Log("Setting Name");
        var data = SessionManager<SessionData>.Instance.GetPlayerData(clientId);
        if (data == null) return;
        data.PlayerName = name;
        SessionManager<SessionData>.Instance.SetPlayerData(clientId, data);
        Debug.Log(SessionManager<SessionData>.Instance.GetPlayerData(clientId).PlayerName);
    }


    public override void OnDestroy()
    {
        // Clean up callbacks
        if(NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        base.OnDestroy();
    }
}
