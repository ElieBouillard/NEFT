using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using Steamworks;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class NetworkClientMessage : MonoBehaviour
{
    internal enum  MessageId : ushort
    {
        clientConnected = 1,
        startGame,
        movement,
        rotation,
        animation,
        shoot,
        shootReceived,
    }

    #region Sended
    public void SendOnClientConnected()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.clientConnected);
        if (NetworkManager.Instance.UseSteam) message.AddULong((ulong)SteamUser.GetSteamID());
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.startGame);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnMovement(Vector3 pos)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.movement);
        message.AddVector3(pos);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnRotation(float y)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.rotation);
        message.AddFloat(y);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnAnimation(float velocityX, float velocityZ)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.animation);
        message.AddFloat(velocityX);
        message.AddFloat(velocityZ);
        NetworkManager.Instance.Client.Send(message);
    }
    
    public void SendOnShoot(PlayerFireController.Shoot  shoot)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.shoot);
        message.AddInt(shoot.HitId);
        message.AddInt(shoot.Type);
        message.AddVector3(shoot.Pos);
        message.AddVector3(shoot.Dir);
        message.AddUShort(shoot.PlayerHitId);
        NetworkManager.Instance.Client.Send(message);
    }

    public static void SendOnShootReceived(ushort idToSent, PlayerFireController.Shoot shoot)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.shootReceived);
        message.AddUShort(idToSent);
        message.AddInt(shoot.HitId);
        NetworkManager.Instance.Client.Send(message);
    }
    #endregion


    #region Received

    [MessageHandler((ushort) NetworkServerMessage.MessageId.spawnLobbyPlayer)]
    private static void OnSpawnLobbyPlayer(Message message)
    {
        ushort playerId = message.GetUShort();
        ulong steamId = new ulong();
        if (NetworkManager.Instance.UseSteam) steamId = message.GetULong();
        NetworkManager.Instance.SpawnLobbyPlayer(playerId , steamId);
    }

    [MessageHandler((ushort) NetworkServerMessage.MessageId.despawnLobbyPlayer)]
    private static void OnDespawnLobbyPlayer(Message message)
    {
        ushort playerId = message.GetUShort();
        NetworkManager.Instance.DespawnLobbyPlayer(playerId);
    }

    [MessageHandler((ushort) NetworkServerMessage.MessageId.startGame)]
    private static void OnServerStartGame(Message message)
    {
        NetworkManager.Instance.OnStartGame();
    }

    [MessageHandler((ushort) NetworkServerMessage.MessageId.movement)]
    private static void OnServerMovePlayer(Message message)
    {
        ushort playerId = message.GetUShort();
        Vector3 pos = message.GetVector3();

        foreach (var id in NetworkManager.Instance.Players.Keys)
        {
            if (id == playerId)
            {
                NetworkManager.Instance.Players[id].Move(pos);
            }
        }
    }

    [MessageHandler((ushort) NetworkServerMessage.MessageId.rotation)]
    private static void OnServerRotatePlayer(Message message)
    {
        ushort playerId = message.GetUShort();
        float y = message.GetFloat();

        foreach (var id in NetworkManager.Instance.Players.Keys)
        {
            if (id == playerId)
            {
                NetworkManager.Instance.Players[id].Rotate(y);
            }
        }
    }

    [MessageHandler((ushort) NetworkServerMessage.MessageId.animation)]
    private static void OnServerAnimatePlayer(Message message)
    {
        ushort playerId = message.GetUShort();
        float velocityX = message.GetFloat();
        float velocityZ = message.GetFloat();

        foreach (var id in NetworkManager.Instance.Players.Keys)
        {
            if (id == playerId)
            {
                NetworkManager.Instance.Players[id].SetAnimation(velocityX, velocityZ);
            }
        }

    }

    private static List<int> _shotsIdReceived = new List<int>();

    [MessageHandler((ushort) NetworkServerMessage.MessageId.shoot)]
    private static void OnServerClientShoot(Message message)
    {
        ushort playerId = message.GetUShort();
        int hitId = message.GetInt();
        int type = message.GetInt();
        Vector3 pos = message.GetVector3();
        Vector3 dir = message.GetVector3();
        ushort playerHit = message.GetUShort();

        if (_shotsIdReceived.Contains(hitId)) return;
        _shotsIdReceived.Add(hitId);
        
        PlayerFireController.Shoot shoot = new PlayerFireController.Shoot(type, pos, dir, playerHit);
        shoot.HitId = hitId;

        foreach (var player in NetworkManager.Instance.Players)
        {
            if (player.Value.PlayerId == playerId)
            {
                player.Value._fireController.MakeAShoot(shoot);
            }

            if (player.Value.PlayerId == playerHit)
            {
                player.Value._healthController.TakeDamage();
            }
        }
        
        Debug.Log($"{playerId} : {hitId} : {type} : {pos} : {dir} : {playerHit}");
        
        SendOnShootReceived(playerId, shoot);
    }

    [MessageHandler((ushort) NetworkServerMessage.MessageId.shootReceived)]
    private static void OnServerClientShootReceived(Message message)
    {
        int shootId = message.GetInt();
        NetworkManager.Instance.LocalPlayer._fireController.ReceivedShoot(shootId);
    }
    #endregion
}
