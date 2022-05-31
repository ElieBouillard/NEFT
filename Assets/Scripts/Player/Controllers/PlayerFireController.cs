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

            Ray ray = new Ray(_shootPos.position, _shootPos.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                Debug.DrawRay(hit.point, hit.normal * 3f, Color.green, 5f);
                
                GameObject _impactInstance = Instantiate(_impactParticleSystem, hit.point, Quaternion.identity);
                _impactInstance.transform.forward = hit.normal;

                if (hit.transform.TryGetComponent<PlayerIdentity>(out PlayerIdentity playerHit))
                {
                    playerHit._healthController.TakeDamage();
                    SendShoot(2, hit.point, playerHit.PlayerId);
                }
                else
                {
                    SendShoot(1, hit.point, new ushort());
                }
            }
            else
            {
                SendShoot(0,Vector3.zero, new ushort());
            }
        }
    }

    public void SendShoot(int hit, Vector3 pos, ushort playerHitId)
    {
        NetworkManager.Instance.ClientMessage.SendOnShoot(hit, pos, playerHitId);
    }

    public void ReceivedShoot(int hit, Vector3 pos, ushort playerHit)
    {
        _muzzleFlashParticleSystem.Stop();
        _muzzleFlashParticleSystem.Play();

        if (hit == 0) return;
        
        GameObject _impactInstance = Instantiate(_impactParticleSystem, pos, Quaternion.identity);
        // _impactInstance.transform.forward = hit.normal;
            
        if (hit == 1) return;

        foreach (ushort id in NetworkManager.Instance.Players.Keys)
        {
            if (id == playerHit)
            {
                Debug.Log($"{NetworkManager.Instance.Players[id].gameObject.name} hit");
                NetworkManager.Instance.Players[id]._healthController.TakeDamage();
            }
        }
    }
}
