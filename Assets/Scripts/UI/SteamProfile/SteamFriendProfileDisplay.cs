using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamFriendProfileDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _profileText;
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private Button _inviteButton;

    public CSteamID FriendId;

    private void Start()
    {
        _inviteButton.onClick.AddListener(InviteFriend);
    }

    public void SetProfileName(string s)
    {
        _profileText.text = s;
    }

    
    public void SetProfilePicture(Texture t)
    {
        _profileImage.texture = t;
    }

    private void InviteFriend()
    {
        SteamMatchmaking.InviteUserToLobby(SteamLobbyManager.Instance.GetLobbyId(), FriendId);
    }
}
