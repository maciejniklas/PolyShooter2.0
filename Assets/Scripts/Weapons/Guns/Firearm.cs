using UnityEngine;
using Weapons.Interfaces;

namespace Weapons.Guns
{
    public class Firearm : MonoBehaviour, IWeapon
    {
        [SerializeField] private float damage = 10f;

        public float Damage => damage;
        public GameObject Instance => gameObject;
        public GameObject Owner { get; set; }
        
        public void Attack()
        {
            Debug.Log("Attack");
        }

        public event OnWeaponAttackEventHandler OnWeaponAttack;
    }
}