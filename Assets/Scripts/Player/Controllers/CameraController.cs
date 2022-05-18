using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Singleton
    private static CameraController _instance;
    public static CameraController Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(CameraController)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothSpeed;
    [SerializeField] private Transform _target;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void FixedUpdate()
    {
        if (_target == null) return;

        var step = _smoothSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, _offset + _target.position, step);
    }
}
