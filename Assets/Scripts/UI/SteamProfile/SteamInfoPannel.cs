using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamInfoPannel : MonoBehaviour
{
    [SerializeField] private RawImage _steamProfileImage;
    [SerializeField] private TMP_Text _steamProfileText;
    [SerializeField] private GameObject _friendsScrollView;
    [SerializeField] private SteamFriendProfileDisplay _steamFriendProfile;
    
    protected Callback<AvatarImageLoaded_t> ImageLoaded;
    protected Callback<LobbyInvite_t> InviteLobby;

    private CSteamID _playerId;

    private Dictionary<CSteamID, SteamFriendProfileDisplay> _friends = new Dictionary<CSteamID, SteamFriendProfileDisplay>();
    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnPlayerAvatarLoaded);
        InviteLobby = Callback<LobbyInvite_t>.Create(OnInvite);
        
        _playerId = SteamUser.GetSteamID();

        _steamProfileText.text = SteamFriends.GetPersonaName();
        
        LoadPlayerAvatar();
        
        LoadFriends();
    }

    private float clock = 5f;
    private void Update()
    {
        clock += Time.deltaTime;

        if (clock > 3f)
        {
            LoadFriends();
            clock = 0f;
        }
    }

    public void LoadPlayerAvatar()
    {
        int _playerAvatarId = SteamFriends.GetLargeFriendAvatar(_playerId);
        
        if(_playerAvatarId == -1)  {Debug.Log("Error loading image"); return;}

        _steamProfileImage.texture = GetSteamImageAsTexture(_playerAvatarId);
    }


    private float yPosDisplay = 350f;
    public void LoadFriends()
    {
        int friendNb = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);

        SteamFriends.GetFriendGamePlayed(_playerId, out FriendGameInfo_t f);

        CGameID gameId = f.m_gameID;

        for (int i = 0; i < friendNb; i++)
        {
            CSteamID friend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
            SteamFriends.GetFriendGamePlayed(friend, out FriendGameInfo_t gameParty);

            if (gameId == gameParty.m_gameID)
            {
                SteamFriendProfileDisplay display = Instantiate(_steamFriendProfile, _friendsScrollView.transform);
                display.gameObject.GetComponent<RectTransform>().position = new Vector3(
                    display.gameObject.GetComponent<RectTransform>().position.x, yPosDisplay,
                    display.gameObject.GetComponent<RectTransform>().position.z);
                yPosDisplay -= 125f;
                _friends.Add(friend, display);

                display.FriendId = friend;

                display.SetProfileName(SteamFriends.GetFriendPersonaName(friend));

                Debug.Log(SteamFriends.GetFriendPersonaName(friend));

                int _friendAvatarId = SteamFriends.GetLargeFriendAvatar(friend);

                if (_friendAvatarId != -1)
                {
                    display.SetProfilePicture(GetSteamImageAsTexture(_friendAvatarId));
                }
            }
        }
    }
    
    private void OnPlayerAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == (ulong)_playerId)
        { 
            _steamProfileImage.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            foreach (var id in _friends.Keys)
            {
                if (callback.m_steamID.m_SteamID == (ulong) id)
                {
                    _friends[id].SetProfilePicture(GetSteamImageAsTexture(callback.m_iImage));
                }
            } 
        }

    }
    
    private void OnInvite(LobbyInvite_t param)
    {
        Debug.Log(param.m_ulSteamIDUser);
    }
    
    private Texture2D GetSteamImageAsTexture(int image)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(image, out uint width, out uint height);

        if (isValid)
        {
            byte[] imageTemp = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(image, imageTemp,(int)width * (int)height * 4);

            if (isValid)
            {
                texture = new Texture2D((int) width, (int) height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(imageTemp);
                texture.Apply();
            }
        }
        return texture;
    }
}
