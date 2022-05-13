using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager _instance;
    public static UIManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(UIManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    private ConnectionPannel _connectionPannel;
    private LobbyPannel _lobbyPannel;

    private void Awake()
    {
        Instance = this;
        
        _connectionPannel = GetComponentInChildren<ConnectionPannel>();
        _lobbyPannel = GetComponentInChildren<LobbyPannel>();
        
        _connectionPannel.gameObject.SetActive(true);
        _lobbyPannel.gameObject.SetActive(false);
    }

    public void ClientConnected(bool value)
    {
        _connectionPannel.gameObject.SetActive(!value);
        _lobbyPannel.gameObject.SetActive(value);
    }
}
