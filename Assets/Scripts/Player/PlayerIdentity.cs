using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    public ushort PlayerId;
    public bool IsLocalPlayer { get; private set; }

    public ulong SteamPlayerId;

    public string SteamPlayerName;

    public void LoadSteamInfo(ulong steamId)
    {
        SteamPlayerId = steamId;
        SteamPlayerName = SteamFriends.GetFriendPersonaName((CSteamID)SteamPlayerId);
        gameObject.name = $"Player {PlayerId} : {SteamPlayerName}";

        PlayerLobbyDisplay lobbyDisplay = gameObject.GetComponent<PlayerLobbyDisplay>() ;
        lobbyDisplay.SetPlayerNameText();
        lobbyDisplay.LoadPlayerAvatar();
    }
    
    public void SetPlayerAsLocalPlayer()
    {
        IsLocalPlayer = true;
        gameObject.GetComponentInChildren<Renderer>().material.color = Color.green;
    }
}
