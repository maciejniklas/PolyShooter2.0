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
        Transform LookAtPointIfRemoteInstance { set; }
        GameObject Owner { get; set; }

        void Attack();

        event OnWeaponAttackEventHandler OnWeaponAttack;
    }

    public delegate void OnWeaponAttackEventHandler();
}