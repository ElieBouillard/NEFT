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

    public Animator _animator;
    

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
    private float? _targetRotY;
    private Vector2? _targetAnimation;
    private void Update()
    {
        if(IsLocalPlayer) return;
        
        if(_targetPos != null) transform.position = Vector3.LerpUnclamped(transform.position, _targetPos.Value, Time.deltaTime * 20f);
        if (_targetRotY != null)
        {
            Quaternion targetRotation = new Quaternion();
            targetRotation = Quaternion.Euler(new Vector3(0,_targetRotY.Value,0));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 25f);
        }
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
        _targetRotY = rotY;
    }

    public void SetAnimation(float velocityX, float velocityZ)
    {
        if(_animator == null) return;
        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
    }
}
