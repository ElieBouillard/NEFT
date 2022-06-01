using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPannel : Pannel
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        _joinButton.gameObject.SetActive(!NetworkManager.Instance.UseSteam);
    }

    public override void AssignButtonsReferences()
    {
        _hostButton.onClick.AddListener(OnClickHostButton);
        _joinButton.onClick.AddListener(OnClickJoinButton);
        _quitButton.onClick.AddListener(OnClickQuitButton);
    }

    private void OnClickHostButton()
    {
        NetworkManager.Instance.StartHost();
    }

    private void OnClickJoinButton()
    {
        NetworkManager.Instance.JoinLobby();
    }

    private void OnClickQuitButton()
    {
        Application.Quit();
    }
}
