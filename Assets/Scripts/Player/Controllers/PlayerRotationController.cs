using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationController : MonoBehaviour
{
    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private float _smoothForce = 25f;
    
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
        {
            Vector3 dir = new Vector3(hit.point.x - transform.position.x, 0f, hit.point.z - transform.position.z).normalized;
            
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * _smoothForce);
        }
    }

    private int index = 0;
    private void FixedUpdate()
    {
        if (NetworkManager.Instance == null) return;
        
        if (index % 3 == 0)
        {
            NetworkManager.Instance.ClientMessage.SendOnRotation(transform.rotation.eulerAngles.y);
            index = 0;
        }
        index++;
    }
}
