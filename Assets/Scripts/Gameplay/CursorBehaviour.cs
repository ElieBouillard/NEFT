using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBehaviour : MonoBehaviour
{
    #region Singleton
    private static CursorBehaviour _instance;
    public static CursorBehaviour Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(CursorBehaviour)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    [SerializeField] private bool _showCursor = true;

    private void Start()
    {
        Cursor.visible = _showCursor;
    }
}
