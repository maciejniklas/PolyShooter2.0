using System.Collections;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.VFX;
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

        [Header("VFX")]
        [SerializeField] protected VisualEffect muzzleFlashVFX;

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

        private bool _isReloading;
        
        private static readonly int ReloadAnimationTrigger = Animator.StringToHash("Reload");

        protected virtual void Awake()
        {
            BulletsInMagazine = BulletsPerMagazine;
            Magazines = initialMagazines;

            IsAbleToShoot = true;

            _isReloading = false;
        }

        protected virtual void Update()
        {
            if (!photonView.IsMine)
            {
                transform.LookAt(shotStartPoint.position + shotStartPoint.forward * Range);
            }
            else
            {
                if (Input.GetButtonDown("Reload")) photonView.RPC(nameof(Reload), RpcTarget.All);
            }
        }

        public virtual void Attack()
        {
            OnShot?.Invoke(BulletsInMagazine);
            OnWeaponAttack?.Invoke(this);
        }

        public IEnumerator FireRateCooldown()
        {
            IsAbleToShoot = false;

            yield return new WaitForSeconds(1 / FireRate);

            IsAbleToShoot = true;
        }

        [PunRPC]
        public void Reload()
        {
            while (true)
            {
                if (Magazines <= 0)
                {
                    if (photonView.IsMine) Notification.ErrorMessage("No more magazines!");
                    return;
                }
            
                // Raise animation trigger
                if (!_isReloading)
                {
                    _isReloading = true;
                    Owner.GetComponent<Animator>().SetTrigger(ReloadAnimationTrigger);
                    continue;
                }

                _isReloading = false;
                
                // Called during animation
                if (photonView.IsMine)
                {
                    Magazines -= 1;
                    BulletsInMagazine = BulletsPerMagazine;
                    OnReload?.Invoke(BulletsInMagazine, Magazines);
                }
            
                break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * range);
        }
    }
}