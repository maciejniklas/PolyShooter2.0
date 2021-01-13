using UnityEngine;
using Weapons.Interfaces;

namespace Characters.Interfaces
{
    /// <summary>
    /// Implements behaviours that enable to equipping items
    /// </summary>
    public interface IAbleToEquip
    {
        IWeapon EquippedWeapon { get; }
        
        void EquipWeapon(IWeapon weapon);
    }
}