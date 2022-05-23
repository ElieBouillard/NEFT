using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBehaviour : MonoBehaviour
{
    [SerializeField] private bool _showCursor = true;

    private void Start()
    {
        Cursor.visible = _showCursor;
    }
}
