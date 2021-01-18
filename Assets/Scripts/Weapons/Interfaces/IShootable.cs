using System.Collections;

namespace Weapons.Interfaces
{
    /// <summary>
    /// Implements behaviour of shootable weapons
    /// </summary>
    public interface IShootable
    {
        int BulletsInMagazine { get; }
        int BulletsPerMagazine { get; }
        float FireRate { get; }
        int Magazines { get; }
        float Range { get; }

        IEnumerator FireRateCooldown();
        void Reload();

        event OnReloadEventHandler OnReload;
        event OnShotEventHandler OnShot;
    }

    public delegate void OnReloadEventHandler(int bulletsInMagazine, int magazines);
    public delegate void OnShotEventHandler(int bulletsInMagazine);
}