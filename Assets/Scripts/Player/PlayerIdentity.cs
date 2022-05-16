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
    private float? _targetRot;
    private void Update()
    {
        if(IsLocalPlayer) return;
        
        if(_targetPos != null) transform.position = Vector3.LerpUnclamped(transform.position, _targetPos.Value, Time.deltaTime * 20f);
        if (_targetRot != null) transform.rotation = Quaternion.Euler(new Vector3(0,  Mathf.Lerp(transform.rotation.eulerAngles.y, _targetRot.Value, Time.deltaTime * 25f)));
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

    public void Rotate(float rotY)
    {
        _targetRot = rotY;
    }
}
