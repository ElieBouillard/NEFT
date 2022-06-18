using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    
    [SerializeField] private GameObject _pausePannel;

    private bool _isPause;

    private void Awake()
    {
        _isPause = false;
        _pausePannel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isPause = !_isPause;
            _pausePannel.SetActive(_isPause);
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.LocalPlayer.GetComponent<CMF.CharacterKeyboardInput>().EnableInput = !_isPause;
            }
        }
    }
}
