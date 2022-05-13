using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPannel : Pannel
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;

    public override void AssignButtonsReferences()
    {
        _hostButton.onClick.AddListener(OnClickHostButton);
        _joinButton.onClick.AddListener(OnClickJoinButton);
    }

    private void OnClickHostButton()
    {
        NetworkManager.Instance.StartHost();
    }

    private void OnClickJoinButton()
    {
        NetworkManager.Instance.JoinLobby();
    }
}
