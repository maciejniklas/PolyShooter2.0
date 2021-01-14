﻿using System.Collections;
using Photon.Pun;
using UI;
using UnityEngine;
using Weapons.Interfaces;

namespace Weapons.Guns
{
    public abstract class Firearm : MonoBehaviourPun, IWeapon, IShootable
    {
        [Header("General")]
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected int bulletsPerMagazine;
        [Tooltip("Describes how many shots are allowed per one second.")]
        [SerializeField] protected float fireRate;
        [SerializeField] protected int initialMagazines;
        [SerializeField] protected float range;

        [HideInInspector]
        public Transform shotStartPoint;

        public int BulletsInMagazine { get; protected set; }
        public int BulletsPerMagazine => bulletsPerMagazine;
        public float Damage => damage;
        public float FireRate => fireRate;
        public GameObject Instance => gameObject;
        public int Magazines { get; private set; }
        public GameObject Owner { get; set; }
        public float Range => range;
        
        public event OnReloadEventHandler OnReload;
        public event OnShotEventHandler OnShot;
        public event OnWeaponAttackEventHandler OnWeaponAttack;

        protected bool IsAbleToShoot;
        protected RaycastHit Hit;

        protected virtual void Awake()
        {
            BulletsInMagazine = BulletsPerMagazine;
            Magazines = initialMagazines;

            IsAbleToShoot = true;
        }

        protected virtual void Update()
        {
            if (!photonView.IsMine)
            {
                transform.LookAt(shotStartPoint.position + shotStartPoint.forward * Range);
            }
            else
            {
                if (Input.GetButtonDown("Reload")) Reload();
            }
        }

        public virtual void Attack()
        {
            OnShot?.Invoke(BulletsInMagazine);
        }

        public IEnumerator FireRateCooldown()
        {
            IsAbleToShoot = false;

            yield return new WaitForSeconds(1 / FireRate);

            IsAbleToShoot = true;
        }

        public void Reload()
        {
            if (!photonView.IsMine) return;

            if (Magazines <= 0)
            {
                Notification.Instance.ErrorMessage("No more magazines!");
                return;
            }

            Magazines -= 1;
            BulletsInMagazine = BulletsPerMagazine;
            
            OnReload?.Invoke(BulletsInMagazine, Magazines);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * range);
        }
    }
}