using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _muzzleFlashParticleSystem; 
    private Animator _animator;
    

    private void Awake()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetBool("Fire", true);
            _muzzleFlashParticleSystem.Stop();
            _muzzleFlashParticleSystem.Play();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _animator.SetBool("Fire", false);
            
        }
    }
}
