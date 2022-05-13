using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPannel : Pannel
{
    #region Singleton
    private static LobbyPannel _instance;
    public static LobbyPannel Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(LobbyPannel)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    [SerializeField] private Button _startGame;
    [SerializeField] private Button _leaveLobby;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void OnEnable()
    {
        _startGame.gameObject.SetActive(false);
    }

    public void EnableHostStartGameButton()
    {
        _startGame.gameObject.SetActive(true);
    }
    
    public override void AssignButtonsReferences()
    {
        _startGame.onClick.AddListener(OnClickStartGameButton);
        _leaveLobby.onClick.AddListener(OnClickLeaveLobbyButton);
    }

    private void OnClickStartGameButton()
    {
        NetworkManager.Instance.StartGame();
    }

    private void OnClickLeaveLobbyButton()
    {
        NetworkManager.Instance.LeaveLobby();
    }
}
