using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerFireController : MonoBehaviour
{
    [SerializeField] private Transform _shootPos;
    [SerializeField] private ParticleSystem _muzzleFlashParticleSystem;
    [SerializeField] private GameObject _impactParticleSystem;

    private PlayerIdentity _player;

    private void Awake()
    {
        _player = GetComponent<PlayerIdentity>();
    }

    private void Update()
    {
        if (!_player.IsLocalPlayer) return;
     
        if (Input.GetMouseButtonDown(0))
        {
            _muzzleFlashParticleSystem.Stop();
            _muzzleFlashParticleSystem.Play();

            SendShoot(_shootPos.position, _shootPos.forward);
            
            Ray ray = new Ray(_shootPos.position, _shootPos.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                GameObject _impactInstance = Instantiate(_impactParticleSystem, hit.point, Quaternion.identity);
                _impactInstance.transform.forward = hit.normal;

                Debug.DrawRay(hit.point, hit.normal * 3f, Color.green, 5f);
            }
        }
    }

    public void SendShoot(Vector3 pos, Vector3 dir)
    {
        NetworkManager.Instance.ClientMessage.SendOnShoot(pos, dir);
    }

    public void ReceivedShoot(Vector3 pos, Vector3 dir)
    {
        _muzzleFlashParticleSystem.Stop();
        _muzzleFlashParticleSystem.Play();

        Ray ray = new Ray(pos, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
        {
            GameObject _impactInstance = Instantiate(_impactParticleSystem, hit.point, Quaternion.identity);
            _impactInstance.transform.forward = hit.normal;

            Debug.DrawRay(hit.point, hit.normal * 3f, Color.green, 5f);
        }
    }
}
