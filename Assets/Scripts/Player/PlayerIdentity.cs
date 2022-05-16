using System;
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

    private Vector3? _targetPos;
    private void Update()
    {
        if(IsLocalPlayer) return;
        if(_targetPos == null) return;
        transform.position = Vector3.LerpUnclamped(transform.position, _targetPos.Value, Time.deltaTime * 20f);
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) return;
        NetworkManager.Instance.ClientMessage.SendOnMovement(transform.position);
    }

    public void Move(Vector3 pos)
    {
        _targetPos = pos;
    }
}
