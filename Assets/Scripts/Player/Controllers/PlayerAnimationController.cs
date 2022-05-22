using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator;
    private CharacterInput _inputs;

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _inputs = gameObject.GetComponentInParent<CharacterInput>();
    }

    private void Update()
    {
        Vector3 input = new Vector3(_inputs.GetHorizontalMovementInput(), 0f , _inputs.GetVerticalMovementInput());
        
        float velocityZ = Vector3.Dot(input.normalized, transform.forward);
        float velocityX = Vector3.Dot(input.normalized, transform.right);
        
        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);

        NetworkManager.Instance.ClientMessage.SendOnAnimation(velocityX, velocityZ);
    }
}
