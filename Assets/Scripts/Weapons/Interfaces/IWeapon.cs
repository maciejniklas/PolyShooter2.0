using UnityEngine;

namespace Weapons.Interfaces
{
    /// <summary>
    /// Implements weapon behaviour
    /// </summary>
    public interface IWeapon
    {
        float Damage { get; }
        GameObject Instance { get; }
        GameObject Owner { get; set; }

        void Attack();

        event OnWeaponAttackEventHandler OnWeaponAttack;
    }

    public delegate void OnWeaponAttackEventHandler();
}