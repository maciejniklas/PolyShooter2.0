using System;
using System.Collections;
using Characters.Player;
using Masters;
using UnityEngine;
using UnityEngine.UI;
using Weapons.Interfaces;

namespace UI
{
    public class ShootingWeaponHud : MonoBehaviour
    {
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private Text bulletsInMagazineText;
        [SerializeField] private Text magazinesText;
        
        public static ShootingWeaponHud Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(AddDestroyListener());
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void OnWeaponEquip(IWeapon weapon)
        {
            var shootableWeapon = weapon.Instance.GetComponent<IShootable>();

            if (shootableWeapon == null)
            {
                hudPanel.SetActive(false);
            }
            else
            {
                hudPanel.SetActive(true);
                
                bulletsInMagazineText.text = shootableWeapon.BulletsInMagazine.ToString();
                magazinesText.text = shootableWeapon.Magazines.ToString();

                shootableWeapon.OnReload += OnReload;
                shootableWeapon.OnShot += OnShot;
            }
        }

        private IEnumerator AddDestroyListener()
        {
            yield return new WaitUntil(() => PlayerModule.LocalPlayer != null);
            yield return new WaitUntil(() => LevelMaster.Instance != null);

            if (!LevelMaster.Instance.IsSandbox)
            {
                PlayerModule.LocalPlayer.OnDeath += () => Destroy(gameObject);
            }
        }

        private void OnReload(int bulletsInMagazine, int magazines)
        {
            bulletsInMagazineText.text = bulletsInMagazine.ToString();
            magazinesText.text = magazines.ToString();
        }

        private void OnShot(int bulletsInMagazine)
        {
            bulletsInMagazineText.text = bulletsInMagazine.ToString();
        }
    }
}