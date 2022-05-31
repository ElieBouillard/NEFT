using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;

public class NetworkServerMessage : MonoBehaviour
{
    internal enum  MessageId : ushort
    {
        spawnLobbyPlayer = 1,
        despawnLobbyPlayer,
        startGame,
        movement,
        rotation,
        animation,
        shoot,
    }

    #region Sended
    private static void ServerSendOnClientSpawnLobbyPlayer(ushort currId, ulong currSteamId)
    {
        //Display the other for the new one
        foreach (var id in NetworkManager.Instance.Players.Keys)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageId.spawnLobbyPlayer);
            message.AddUShort(id);
            message.AddULong(NetworkManager.Instance.Players[id].SteamPlayerId);
            NetworkManager.Instance.Server.Send(message, currId);
        }
        
        //Display the new one for the other and the new one
        Message newMessage = Message.Create(MessageSendMode.reliable, MessageId.spawnLobbyPlayer);
        newMessage.AddUShort(currId);
        newMessage.AddULong(currSteamId);
        NetworkManager.Instance.Server.SendToAll(newMessage);
    }

    public void ServerSendOnClientDespawnLobbyPlayer(ushort currId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.despawnLobbyPlayer);
        message.AddUShort(currId);
        NetworkManager.Instance.Server.SendToAll(message, currId);
    }

    private static void ServerSendOnClientStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.startGame);
        NetworkManager.Instance.Server.SendToAll(message);
    }

    private static void ServerSendClientMovement(ushort id, Vector3 pos)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.movement);
        message.AddUShort(id);
        message.AddVector3(pos);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }

    private static void ServerOnClientRotation(ushort id, float y)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.rotation);
        message.AddUShort(id);
        message.AddFloat(y);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }

    private static void ServerOnClientAnimation(ushort id, Message message)
    {
       Message newMessage = Message.Create(MessageSendMode.unreliable, MessageId.animation);
       newMessage.AddUShort(id);
       newMessage.AddFloat(message.GetFloat());
       newMessage.AddFloat(message.GetFloat());
       NetworkManager.Instance.Server.SendToAll(newMessage, id);
    }
    
    private static void ServerOnClientShoot(ushort id, Message message)
    {
        Message newMessage = Message.Create(MessageSendMode.reliable, MessageId.shoot);
        newMessage.AddUShort(id);
        newMessage.AddInt(message.GetInt());
        newMessage.AddVector3(message.GetVector3());
        newMessage.AddUShort(message.GetUShort());
        NetworkManager.Instance.Server.SendToAll(newMessage, id);
    }
    #endregion


    #region Received
    [MessageHandler((ushort) NetworkClientMessage.MessageId.clientConnected)]
    private static void OnClientConnected(ushort id, Message message)
    {
        ulong steamId = new ulong();
        if (NetworkManager.Instance.UseSteam) steamId = message.GetULong();
        ServerSendOnClientSpawnLobbyPlayer(id, steamId);   
    }

    [MessageHandler((ushort) NetworkClientMessage.MessageId.startGame)]
    private static void OnClientStartGame(ushort id, Message message)
    {
        if (id != 1) return; //Check if client is host
        ServerSendOnClientStartGame();
    }

    [MessageHandler((ushort) NetworkClientMessage.MessageId.movement)]
    private static void OnClientMovement(ushort id, Message message)
    {
        ServerSendClientMovement(id, message.GetVector3());
    }

    [MessageHandler((ushort) NetworkClientMessage.MessageId.rotation)]
    private static void OnClientRotation(ushort id, Message message)
    {
        ServerOnClientRotation(id, message.GetFloat());
    }

    [MessageHandler((ushort) NetworkClientMessage.MessageId.animation)]
    private static void OnClientAnimation(ushort id, Message message)
    {
        ServerOnClientAnimation(id, message);
    }

    [MessageHandler((ushort) NetworkClientMessage.MessageId.shoot)]
    private static void OnClientShoot(ushort id, Message message)
    {
        ServerOnClientShoot(id, message);
    }
    #endregion
}
