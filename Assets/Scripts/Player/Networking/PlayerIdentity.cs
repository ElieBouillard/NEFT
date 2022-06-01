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

    [SerializeField] private Animator _animator;

    public PlayerFireController _fireController;
    public PlayerHealthController _healthController;

    private void Awake()
    {
        _fireController = gameObject.GetComponent<PlayerFireController>();
        _healthController = gameObject.GetComponent<PlayerHealthController>();
    }

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
        // gameObject.GetComponentInChildren<Renderer>().material.color = Color.green;
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
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }

        if (_targetAnimation != null)
        {
            Vector2 currAnimation = new Vector2(_animator.GetFloat("VelocityX"), _animator.GetFloat("VelocityZ"));
            Vector2 animation = Vector2.Lerp(currAnimation, _targetAnimation.Value, Time.deltaTime * 5f);
            _animator.SetFloat("VelocityX", animation.x);
            _animator.SetFloat("VelocityZ", animation.y);
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
        _targetAnimation = new Vector2(velocityX, velocityZ);
    }
}