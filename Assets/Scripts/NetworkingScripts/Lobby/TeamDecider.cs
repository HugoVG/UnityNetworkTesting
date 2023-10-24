using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TeamDecider : NetworkBehaviour
{
    private NetworkList<ulong> foxPlayers;
    private NetworkList<ulong> badgerPlayers;
    private NetworkList<ulong> unDecidedPlayers;

    [SerializeField] private GameObject FoxContainer;
    [SerializeField] private GameObject BadgerContainer;
    [SerializeField] private GameObject JustJoined;

    private GameObject playerText;

    private void Start()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Spawn();
        }
        foxPlayers = new NetworkList<ulong>(writePerm: NetworkVariableWritePermission.Server);
        badgerPlayers = new NetworkList<ulong>(writePerm: NetworkVariableWritePermission.Server);
        unDecidedPlayers = new NetworkList<ulong>(writePerm: NetworkVariableWritePermission.Server);
        
    }

    public override void OnNetworkSpawn()
    {
        foxPlayers.OnListChanged += (a) => SetSlots();
        badgerPlayers.OnListChanged += (a) => SetSlots();
        base.OnNetworkSpawn();
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetTeamServerRpc(ulong PlayerId, Team team, string name)
    {
        Debug.Log("SetTeamServerRpc");
            
        // Remove the player from the other lists
        if (foxPlayers.Contains(PlayerId))
        {
            badgerPlayers.Remove(PlayerId);
            unDecidedPlayers.Remove(PlayerId);
        }
        else if (badgerPlayers.Contains(PlayerId))
        {
            foxPlayers.Remove(PlayerId);
            unDecidedPlayers.Remove(PlayerId);
        }
        else if (unDecidedPlayers.Contains(PlayerId))
        {
            foxPlayers.Remove(PlayerId);
            badgerPlayers.Remove(PlayerId);
        }
        
        // Add the player to the correct list
        switch (team)
        {
            case Team.Fox:
                foxPlayers.Add(PlayerId);
                break;
            case Team.Badger:
                badgerPlayers.Add(PlayerId);
                break;
            case Team.None:
                unDecidedPlayers.Add(PlayerId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(team), team, null);
        }
        
        // Update the player data
        try
        {
            var data = SessionManager<SessionData>.Instance.GetPlayerData(PlayerId);
            data.PlayerTeam = team;
            SessionManager<SessionData>.Instance.SetPlayerData(PlayerId, data);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            throw;
        }
        
        Debug.Log($"{PlayerId}, {team}, {name}");
        // React to the team change on the client
        SetTeamClientRpc(PlayerId, (int)team, name);
    }
    [ClientRpc]
    private void SetTeamClientRpc(ulong playerId, int team, string name)
    {
        Debug.Log($"{playerId}, {(Team)team}, {name}");
        TextMeshProUGUI[] slots;
        slots = (Team)team == Team.Badger ? BadgerContainer.GetComponentsInChildren<TextMeshProUGUI>() : FoxContainer.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var slot in slots)
        {
            if (slot.text != "") continue;
            RemoveFromSlot(name);
            slot.text = name;
            break;
        }
    }
    private void SetSlots()
    {
        var slots = FoxContainer.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        slots.AddRange(BadgerContainer.GetComponentsInChildren<TextMeshProUGUI>());
        slots.AddRange(JustJoined.GetComponentsInChildren<TextMeshProUGUI>());
        foreach (var slot in slots) // Clear the slots
        {
            slot.text = "";
        }
        var badgerSlots = BadgerContainer.GetComponentsInChildren<TextMeshProUGUI>();
        var foxSlots = FoxContainer.GetComponentsInChildren<TextMeshProUGUI>();
        var justJoinedSlots = JustJoined.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var foxplayer in foxPlayers)
        {
            //fill the fox slots
            var data = SessionManager<SessionData>.Instance.GetPlayerData(foxplayer);
            foreach (var slot in foxSlots)
            {
                if (slot.text != "") continue;
                slot.text = data.PlayerName;
                break;
            }
        }
        foreach (var badgerplayer in badgerPlayers)
        {
            //fill the badger slots
            var data = SessionManager<SessionData>.Instance.GetPlayerData(badgerplayer);
            foreach (var slot in badgerSlots)
            {
                if (slot.text != "") continue;
                slot.text = data.PlayerName;
                break;
            }
        }
        foreach (var undecidedplayer in unDecidedPlayers)
        {
            //fill the undecided slots
            var data = SessionManager<SessionData>.Instance.GetPlayerData(undecidedplayer);
            foreach (var slot in justJoinedSlots)
            {
                if (slot.text != "") continue;
                slot.text = data.PlayerName;
                break;
            }
        }
    }

    public void SetTeam(int team)
    {
        Debug.Log("SetTeam");
        string Playername = PlayerPrefs.GetString("PlayerName", "Unknown Player");
        SetTeamServerRpc(NetworkManager.Singleton.LocalClientId, (Team)team, Playername);
    }

    private void RemoveFromSlot(string PlayerName)
    {
        var slots = FoxContainer.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        slots.AddRange(BadgerContainer.GetComponentsInChildren<TextMeshProUGUI>());
        slots.AddRange(JustJoined.GetComponentsInChildren<TextMeshProUGUI>());
        
        foreach (var slot in slots)
        {
            if (slot.text != PlayerName) continue;
            slot.text = "";
        }
    }
}
[Serializable]
public struct LobbyPlayerData : IEquatable<LobbyPlayerData>, INetworkSerializable
{
    public FixedPlayerName PlayerName;
    public Team PlayerTeam;
    public ulong PlayerNumber;

    public bool Equals(LobbyPlayerData other)
    {
        return PlayerName.Equals(other.PlayerName) && PlayerTeam == other.PlayerTeam && PlayerNumber == other.PlayerNumber;
    }

    public override bool Equals(object obj)
    {
        return obj is LobbyPlayerData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PlayerName, (int)PlayerTeam, PlayerNumber);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out PlayerName);
            reader.ReadValueSafe(out PlayerTeam);
            reader.ReadValueSafe(out PlayerNumber);
        }
        else
        {
            var reader = serializer.GetFastBufferWriter();
            reader.WriteValueSafe(PlayerName);
            reader.WriteValueSafe(PlayerTeam);
        }
    }
}
public struct FixedPlayerName : INetworkSerializable
{
    FixedString32Bytes m_Name;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out m_Name);
        }
        else
        {
            var reader = serializer.GetFastBufferWriter();
            reader.WriteValueSafe(m_Name);
        }
    }

    public override string ToString()
    {
        return m_Name.Value.ToString();
    }

    public static implicit operator string(FixedPlayerName s) => s.ToString();
    public static implicit operator FixedPlayerName(string s) => new FixedPlayerName() { m_Name = new FixedString32Bytes(s) };
}