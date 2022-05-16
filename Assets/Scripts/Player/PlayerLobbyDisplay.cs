using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyDisplay : MonoBehaviour
{
    private PlayerIdentity _player;
    private int _playerAvatarId;
    private RawImage _profileImage;
    private TMP_Text _profileName;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;
    
    private void Awake()
    {
        _player = GetComponent<PlayerIdentity>();
        _profileImage = GetComponentInChildren<RawImage>();
        _profileName = GetComponentInChildren<TMP_Text>();


        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnPlayerAvatarLoaded);
    }

    public void SetPlayerNameText()
    {
        _profileName.text = _player.SteamPlayerName;
    }

    public void LoadPlayerAvatar()
    {
        _playerAvatarId = SteamFriends.GetLargeFriendAvatar(new CSteamID(_player.SteamPlayerId));
        
        if(_playerAvatarId == -1)  {Debug.Log("Error loading image"); return;}

        _profileImage.texture = GetSteamImageAsTexture(_playerAvatarId);
    }

    private void OnPlayerAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != _player.SteamPlayerId) return;
        _profileImage.texture = GetSteamImageAsTexture(callback.m_iImage);
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
