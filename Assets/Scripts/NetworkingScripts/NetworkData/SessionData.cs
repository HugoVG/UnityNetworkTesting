using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This handles session data in case someone loses internet connection and reconnects.
/// </summary>
public class SessionData : ISessionPlayerData
{
    public FixedPlayerName PlayerName;
    public int PlayerNumber;
    public Team PlayerTeam;
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public int AvatarNetworkGuid;
    public bool IsConnected { get; set; }
    public ulong ClientID { get; set; }
    public void Reinitialize()
    {
    }
}

public enum Team
{
    Fox,
    Badger,
    None
}
