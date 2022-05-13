using Steamworks;
using UnityEngine;

public class SteamLobbyManager : MonoBehaviour
{
    #region Singleton
    private static SteamLobbyManager _instance;
    public static SteamLobbyManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(SteamLobbyManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEnter;

    private const string HostAddressKey = "HostAddress";
    private CSteamID lobbyId;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized!");
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    internal void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 5);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            //Lobby Creation failed
            return;
        }

        //Lobby creation succed
        
        lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(lobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());

        NetworkManager.Instance.Server.Start(0, 5);
        NetworkManager.Instance.Client.Connect("127.0.0.1");
    }

    internal void JoinLobby(ulong lobbyId)
    {
        SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkManager.Instance.Server.IsRunning) return;
    
        lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        string hostAddress = SteamMatchmaking.GetLobbyData(lobbyId, HostAddressKey);

        NetworkManager.Instance.Client.Connect(hostAddress);
        
    }

    internal void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(lobbyId);
    }

    public CSteamID GetLobbyId()
    {
        return lobbyId;
    }
}