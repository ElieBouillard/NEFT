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
    #endregion


    #region Received
    [MessageHandler((ushort) NetworkClientMessage.MessageId.clientConnected)]
    private static void OnClientConnected(ushort id, Message message)
    {
        ulong steamId = new ulong();
        if (NetworkManager.Instance.UseSteam) steamId = message.GetULong();
        ServerSendOnClientSpawnLobbyPlayer(id, steamId);   
    }
    #endregion
}
