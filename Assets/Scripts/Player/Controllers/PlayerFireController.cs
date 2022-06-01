using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
            PlayMuzzleFlashFX();

            Ray ray = new Ray(_shootPos.position, _shootPos.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                if (hit.transform.TryGetComponent<PlayerIdentity>(out PlayerIdentity playerHit))
                {
                    playerHit._healthController.TakeDamage();
                    SendShoot(2, hit.point, playerHit.PlayerId);
                }
                else
                {
                    SendShoot(1, hit.point, new ushort());
                }
                
                InstantiateImpact(hit.point, hit.normal);
            }
            else
            {
                SendShoot(0,Vector3.zero, new ushort());
            }
        }
    }

    public void PlayMuzzleFlashFX()
    {
        _muzzleFlashParticleSystem.Stop();
        _muzzleFlashParticleSystem.Play();
    }

    public void InstantiateImpact(Vector3 pos, Vector3 dir)
    {
        GameObject _impactInstance = Instantiate(_impactParticleSystem, pos, Quaternion.identity);
        _impactInstance.transform.forward = dir;
        Destroy(_impactInstance, 4f);
    }
    public void SendShoot(int hit, Vector3 pos, ushort playerHitId)
    {
        NetworkManager.Instance.ClientMessage.SendOnShoot(hit, pos, playerHitId);
    }

    public void ReceivedShoot()
    {
        _player._healthController.TakeDamage();
    }
}
