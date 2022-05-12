using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RiptideDemos.SteamTransport.PlayerHosted;
using RiptideNetworking;
using RiptideNetworking.Transports.SteamTransport;
using RiptideNetworking.Utils;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
    [SerializeField] private bool _useSteam = false;
    [SerializeField] private ushort _port = 7777;
    [SerializeField] private ushort _maxPlayer = 4;
    [SerializeField] public PlayerIdentity PlayerPrefab;
    [SerializeField] public Transform[] SpawnPoints;
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

    private void EnableClient(SteamServer steamServer)
    {
        if (_useSteam) Client = new Client(new SteamClient(steamServer));
        else Client = new Client();

        Client.Connected += ClientOnConnected;
        Client.Disconnected += ClientOnDisconnected;
        Client.ConnectionFailed += ClientOnConnectionFailed;
        Client.ClientConnected += ClientOnPlayerJoin;
        Client.ClientDisconnected += ClientOnPlayerLeft;
    }

    private void EnableServer(SteamServer steamServer)
    {
        if (_useSteam) Server = new Server(steamServer);
        else Server = new Server();

        Server.ClientConnected += ServerOnClientConnected;
        Server.ClientDisconnected += ServerOnClientDisconnected;
    }
    
    #endregion

    #region Server
    private void ServerOnClientConnected(object sender, ServerClientConnectedEventArgs e)
    {
        
    }

    private void ServerOnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        
    }
    #endregion

    #region Client

    private void ClientOnConnected(object sender, EventArgs e)
    {
        
    }

    private void ClientOnDisconnected(object sender, EventArgs e)
    {
        
    }

    private void ClientOnConnectionFailed(object sender, EventArgs e)
    {

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
        if (_useSteam)
        {
            
        }
        else
        {
            Server.Start(_port, _maxPlayer);
            Client.Connect($"127.0.0.1:{_port}");
        }
    }

    public void JoinLobby()
    {
        Client.Connect($"127.0.0.1:{_port}");
    }

    public void LeaveLobby()
    {
        
    }

    public void StartGame()
    {
        
    }
    
    private void CheckForSteamLobby()
    {
        SteamLobbyManager lobbyManager = FindObjectOfType<SteamLobbyManager>();
        
        if (lobbyManager == null) return;
        
        if (_useSteam)
        {
            gameObject.AddComponent<SteamManager>();
            lobbyManager.enabled = true;
        }
        else
        {
            lobbyManager.enabled = false;
        }
    }
    #endregion

    #region Debug

    

    #endregion
}
