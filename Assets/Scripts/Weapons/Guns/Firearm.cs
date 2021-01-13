using System.Collections;
using Photon.Pun;
using UI;
using UnityEngine;
using Weapons.Interfaces;

namespace Weapons.Guns
{
    public class Firearm : MonoBehaviourPun, IWeapon, IShootable
    {
        [Header("General")]
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected int bulletsPerMagazine;
        [Tooltip("Describes how many shots are allowed per one second.")]
        [SerializeField] protected float fireRate;
        [SerializeField] protected int initialMagazines;
        [SerializeField] protected float range;

        public int BulletsInMagazine { get; private set; }
        public int BulletsPerMagazine => bulletsPerMagazine;
        public float Damage => damage;
        public float FireRate => fireRate;
        public GameObject Instance => gameObject;
        public Transform LookAtPointIfRemoteInstance { get; set; }
        public int Magazines { get; private set; }
        public GameObject Owner { get; set; }
        public float Range => range;
        
        public event OnReloadEventHandler OnReload;
        public event OnShotEventHandler OnShot;
        public event OnWeaponAttackEventHandler OnWeaponAttack;

        protected bool _isAbleToShoot;

        protected virtual void Awake()
        {
            BulletsInMagazine = BulletsPerMagazine;
            Magazines = initialMagazines;

            _isAbleToShoot = true;
        }

        protected virtual void Update()
        {
            if (!photonView.IsMine)
            {
                transform.LookAt(LookAtPointIfRemoteInstance);
            }
            else
            {
                if (Input.GetButtonDown("Reload")) Reload();
            }
        }

        public void Attack()
        {
            Debug.Log("Attack");
        }

        public IEnumerator FireRateCooldown()
        {
            _isAbleToShoot = false;

            yield return new WaitForSeconds(1 / FireRate);

            _isAbleToShoot = true;
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
    }
}