using System;
using System.Collections.Generic;
using RiptideNetworking;
using RiptideNetworking.Transports.SteamTransport;
using RiptideNetworking.Utils;
using UnityEngine;

[RequireComponent(typeof(NetworkClientMessage))]
[RequireComponent(typeof(NetworkServerMessage))]
public class NetworkManager : MonoBehaviour
{
    #region Singleton
    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion

    #region Properties
    public Server Server;
    public Client Client;
    [HideInInspector] public NetworkClientMessage ClientMessage;
    [HideInInspector] public NetworkServerMessage ServerMessage;

    public Dictionary<ushort, PlayerIdentity> Players = new Dictionary<ushort, PlayerIdentity>();
    [HideInInspector] public PlayerIdentity LocalPlayer;
    #endregion

    #region Inspector
    [SerializeField] public bool UseSteam = false;
    [SerializeField] private ushort _port = 7777;
    [SerializeField] private ushort _maxPlayer = 4;
    [SerializeField] private PlayerIdentity LobbyPlayerPrefab;
    [SerializeField] private PlayerIdentity PlayerPrefab;
    [SerializeField] private Transform[] SpawnPoints;
    #endregion

    #region Unity
    private void Awake()
    {
        Instance = this;
        
        ClientMessage = GetComponent<NetworkClientMessage>();
        ServerMessage = GetComponent<NetworkServerMessage>();
        
        DontDestroyOnLoad(gameObject);
        
        CheckForSteamLobby();
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        SteamServer steamServer = new SteamServer();
        
        EnableClient(steamServer);
        EnableServer(steamServer);
    }

    private void FixedUpdate()
    {
        Client.Tick();
        
        if(!Server.IsRunning) return;
        
        Server.Tick();
    }

    private void EnableClient(SteamServer steamServer)
    {
        if (UseSteam) Client = new Client(new SteamClient(steamServer));
        else Client = new Client();

        Client.Connected += ClientOnConnected;
        Client.Disconnected += ClientOnDisconnected;
        Client.ConnectionFailed += ClientOnConnectionFailed;
        Client.ClientConnected += ClientOnPlayerJoin;
        Client.ClientDisconnected += ClientOnPlayerLeft;
    }

    private void EnableServer(SteamServer steamServer)
    {
        if (UseSteam) Server = new Server(steamServer);
        else Server = new Server();

        Server.ClientConnected += ServerOnClientConnected;
        Server.ClientDisconnected += ServerOnClientDisconnected;
    }

    private void OnApplicationQuit()
    {
        LeaveLobby();
    }

    #endregion

    #region Server
    private void ServerOnClientConnected(object sender, ServerClientConnectedEventArgs e)
    {

    }

    private void ServerOnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        ServerMessage.ServerSendOnClientDespawnLobbyPlayer(e.Id);
    }
    #endregion

    #region Client
    private void ClientOnConnected(object sender, EventArgs e)
    {
        UIManager.Instance.ClientConnected(true);

        if (Client.Id == 1)
        {
            LobbyPannel.Instance.EnableHostStartGameButton();
        }
        
        ClientMessage.SendOnClientConnected();
    }

    private void ClientOnDisconnected(object sender, EventArgs e)
    {
        UIManager.Instance.ClientConnected(false);
        
        //Destroy and clear players
        foreach (ushort id in Players.Keys)
        {
            Destroy(Players[id].gameObject);
        }
        Players.Clear();
    }

    private void ClientOnConnectionFailed(object sender, EventArgs e)
    {
        UIManager.Instance.ClientConnected(false);
    }

    private void ClientOnPlayerJoin(object sender, ClientConnectedEventArgs e)
    {
        
    }
    
    private void ClientOnPlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        
    }
    
    #endregion
    
    #region Functions

    public void StartHost()
    {
        if (UseSteam)
        {
            SteamLobbyManager.Instance.CreateLobby();
        }
        else
        {
            Server.Start(_port, _maxPlayer);
            Client.Connect($"127.0.0.1:{_port}");
        }
    }

    public void JoinLobby()
    {
        if (UseSteam) return;
        Client.Connect($"127.0.0.1:{_port}");
    }

    public void LeaveLobby()
    {
        Client.Disconnect();
        ClientOnDisconnected(new object(), EventArgs.Empty);
        
        if(UseSteam) SteamLobbyManager.Instance.LeaveLobby();
        
        if(!Server.IsRunning) return;
        Server.Stop();
    }

    public void StartGame()
    {
        
    }
    
    private void CheckForSteamLobby()
    {
        SteamLobbyManager lobbyManager = FindObjectOfType<SteamLobbyManager>();
        
        if (lobbyManager == null) return;
        
        if (UseSteam)
        {
            gameObject.AddComponent<SteamManager>();
            lobbyManager.enabled = true;
        }
        else
        {
            lobbyManager.enabled = false;
        }
    }

    public void SpawnLobbyPlayer(ushort id, ulong steamId)
    {
        PlayerIdentity playerInstance = Instantiate(LobbyPlayerPrefab, SpawnPoints[Players.Count].position, Quaternion.identity);
        playerInstance.PlayerId = id;
        playerInstance.gameObject.name = $"Player {id}";
        Players.Add(id, playerInstance);
        
        if(UseSteam) playerInstance.LoadSteamInfo(steamId);

        if (Client.Id == id)
        {
            LocalPlayer = playerInstance;
            playerInstance.SetPlayerAsLocalPlayer();
        }
    }

    public void DespawnLobbyPlayer(ushort currId)
    {
        foreach (ushort id in Players.Keys)
        {
            if (currId == id)
            {
                Destroy(Players[id].gameObject);
                Players.Remove(currId);
                break;
            }
        }

        //Reorganize players in lobby
        int index = 0;
        foreach (ushort id in Players.Keys)
        {
            Players[id].transform.position = SpawnPoints[index].position;
            index++;
        }
    }
    #endregion

    #region Debug

    [ContextMenu("DebugSteamPlayers")]
    public void DebugSteamPlayers()
    {
        foreach (var id in Players.Keys)
        {
            Debug.Log( $"{Players[id].SteamPlayerName} {Players[id].SteamPlayerId} {id}");
        }
    }

    #endregion
}