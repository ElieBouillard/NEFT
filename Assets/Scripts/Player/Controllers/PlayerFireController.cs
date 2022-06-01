using System;
using System.Collections.Generic;
using CameraShake;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerFireController : MonoBehaviour
{
    [SerializeField] private Transform _shootPos;
    [SerializeField] private ParticleSystem _muzzleFlashParticleSystem;
    [SerializeField] private GameObject _impactParticleSystem;

    private PlayerIdentity _player;

    private List<ShootReceivedData> _shootsToSend = new List<ShootReceivedData>();

    private void Awake()
    {
        _player = GetComponent<PlayerIdentity>();
    }

    private void Update()
    {
        if (!_player.IsLocalPlayer) return;
     
        if (Input.GetMouseButtonDown(0))
        {
            CameraShaker.Presets.ShortShake3D();

            Shoot newShoot = new Shoot(0, Vector3.zero, Vector3.zero, new ushort()); 
            
            Ray ray = new Ray(_shootPos.position, _shootPos.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                newShoot.Pos = hit.point;
                newShoot.Dir = hit.normal;
                
                if (hit.transform.TryGetComponent<PlayerIdentity>(out PlayerIdentity playerHit))
                {
                    playerHit._healthController.TakeDamage();
                    newShoot.PlayerHitId = playerHit.PlayerId;
                    newShoot.Type = 2;
                }
                else
                {
                    newShoot.Type = 1;
                }
            }
            
            MakeAShoot(newShoot);
            _shootsToSend.Add(new ShootReceivedData(newShoot));
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _shootsToSend.Count; i++)
        {
            NetworkManager.Instance.ClientMessage.SendOnShoot(_shootsToSend[i].Shoot);
        }
    }

    public void MakeAShoot(Shoot shoot)
    {
        PlayMuzzleFlashFX();

        if (shoot.Type == 0) return;
        
        InstantiateImpact(shoot);
    }
    
    private void PlayMuzzleFlashFX()
    {
        _muzzleFlashParticleSystem.Stop();
        _muzzleFlashParticleSystem.Play();
    }

    public void InstantiateImpact(Shoot shoot) 
    {
        GameObject _impactInstance = Instantiate(_impactParticleSystem, shoot.Pos, Quaternion.identity);
        _impactInstance.transform.forward = shoot.Dir;
        Destroy(_impactInstance, 4f);
    }

    public void ReceivedShoot(int shootId)
    {
        foreach (var shoot in _shootsToSend)
        {
            if (shoot.Shoot.HitId == shootId)
            {
                shoot.PlayersReceivedCount++;
                if (shoot.PlayersReceivedCount == NetworkManager.Instance.Players.Count - 1)
                {
                    _shootsToSend.Remove(shoot);
                    return;
                }
            }
        }
    }
    
    public struct Shoot
    {
        public int HitId;
        public int Type;
        public Vector3 Pos;
        public Vector3 Dir;
        public ushort PlayerHitId;

        public Shoot(int type, Vector3 pos, Vector3 dir, ushort playerHitId)
        {
            HitId = Random.Range(0, 999999);
            Type = type;
            Pos = pos;
            Dir = dir;
            PlayerHitId = playerHitId;
        }
    }

    public class ShootReceivedData
    {
        public Shoot Shoot;
        public int PlayersReceivedCount;

        public ShootReceivedData(Shoot shoot)
        {
            Shoot = shoot;
            PlayersReceivedCount = 0;
        }
    }
}
