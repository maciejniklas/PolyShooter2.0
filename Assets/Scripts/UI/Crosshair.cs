﻿using System;
using System.Collections;
using Characters.Player;
using Masters;
using UnityEngine;
using UnityEngine.UI;
using Weapons.Interfaces;

namespace UI
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField] private Image fireRateCooldownIndicator;
        [Tooltip("For how many parts cooldown circle will be divided for animation")]
        [SerializeField] private int cooldownFillFactor = 10;

        private void OnEnable()
        {
            StartCoroutine(AddListenerCoroutine());
        }

        private IEnumerator AddListenerCoroutine()
        {
            yield return new WaitUntil(() => PlayerModule.LocalPlayer != null);
            yield return new WaitUntil(() => LevelMaster.Instance != null);

            if (!LevelMaster.Instance.IsSandbox)
            {
                PlayerModule.LocalPlayer.OnDeath += () => Destroy(gameObject);
            }

            PlayerModule.LocalPlayer.EquippedWeapon.OnWeaponAttack += (weapon) =>
            {
                var shootable = weapon.Instance.GetComponent<IShootable>();
                if (shootable == null) return;
                StartCoroutine(StartCooldown(shootable.FireRate));
            };
        }

        private IEnumerator StartCooldown(float fireRate)
        {
            fireRateCooldownIndicator.fillAmount = 1f;
            var cooldownTime = 1 / fireRate;

            for (var index = 1; index <= cooldownFillFactor; index++)
            {
                yield return new WaitForSeconds(cooldownTime / cooldownFillFactor);
                fireRateCooldownIndicator.fillAmount -= 1f / cooldownFillFactor;
            }
        }
    }
}