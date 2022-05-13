using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using Steamworks;
using UnityEngine;

public class NetworkClientMessage : MonoBehaviour
{
    internal enum  MessageId : ushort
    {
        clientConnected = 1,
    }

    #region Sended
    public void SendOnClientConnected()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.clientConnected);
        if (NetworkManager.Instance.UseSteam) message.AddULong((ulong)SteamUser.GetSteamID());
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
    #endregion
}
