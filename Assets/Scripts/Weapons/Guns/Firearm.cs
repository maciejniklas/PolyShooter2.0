using Photon.Pun;
using UnityEngine;
using Weapons.Interfaces;

namespace Weapons.Guns
{
    public class Firearm : MonoBehaviourPun, IWeapon
    {
        [SerializeField] private float damage = 10f;

        public float Damage => damage;
        public GameObject Instance => gameObject;
        public Transform LookAtPointIfRemoteInstance { get; set; }
        public GameObject Owner { get; set; }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                transform.LookAt(LookAtPointIfRemoteInstance);
            }
        }

        public void Attack()
        {
            Debug.Log("Attack");
        }

        public event OnWeaponAttackEventHandler OnWeaponAttack;
    }
}