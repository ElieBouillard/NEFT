using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShaker : MonoBehaviour
{
    #region Singleton
    private static CameraShaker _instance;
    public static CameraShaker Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(CameraShaker)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }
    #endregion
    
    private Vector3 _startPos;
    private Vector3 _shakePos;
    [SerializeField] private float _speed;
    [SerializeField] private float _amount;
    [SerializeField] private float _duration;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _startPos = transform.position;
        _shakePos = transform.position;
    }

    private void Update()
    {
        if (_duration > 0)
        {
            Shake();
            _duration -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _startPos, Time.deltaTime * _speed);
        }
        
        
    }

    public void ShakeCamera(float duration, float amount, float speed)
    {
        _amount = amount;
        _speed = speed;
        _duration = duration;
    }
    
    private void Shake()
    {
        if (transform.position == _shakePos)
        {
            _shakePos = _startPos + new Vector3(Random.Range(-_amount, _amount), Random.Range(-_amount, _amount), 0);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _shakePos, Time.deltaTime * _speed);
        }
    }
}
