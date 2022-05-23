using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLineDisplay : MonoBehaviour
{
    private LineRenderer _line;

    private void Awake()
    {
        _line = gameObject.GetComponent<LineRenderer>();
    }

    private Ray ray;
    private void Start()
    {

    }

    private void Update()
    {
        ray = new Ray(_line.transform.position, _line.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            _line.SetPosition(1, new Vector3(0,0,hit.distance));
        }
        else
        {
            _line.SetPosition(1, new Vector3(0,0,20));
        }
    }
}
