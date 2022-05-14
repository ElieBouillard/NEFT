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
        startGame,
        movement,
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
    #endregion
}
