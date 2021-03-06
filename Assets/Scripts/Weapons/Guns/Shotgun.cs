﻿using System.Collections.Generic;
using Characters.Interfaces;
using Photon.Pun;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons.Guns
{
    public class Shotgun : Firearm
    {
        [Header("Shotgun specific")]
        [SerializeField] private int shellsPerShot = 10;
        [Tooltip("Describes how precise shot could be")]
        [SerializeField] [Range(2, 30)] private float shellsDispersion = 7;

        private List<Vector2> _pointsOnCircle;
        
        protected override void Update()
        {
            base.Update();

            if (!photonView.IsMine) return;
            if (Input.GetButtonDown("Fire1")) Attack();
        }

        [PunRPC]
        public override void Attack()
        {
            if (photonView.IsMine)
            {
                if (!IsAbleToShoot) return;
                
                if (BulletsInMagazine <= 0)
                {
                    Notification.ErrorMessage("No more bullets!");
                    return;
                }

                BulletsInMagazine -= 1;
                
                photonView.RPC(nameof(Attack), RpcTarget.Others);

                StartCoroutine(FireRateCooldown());
                
                // Notify listeners about shot
                base.Attack();
            }
            
            muzzleFlashVFX.Stop();
            muzzleFlashVFX.Play();

            for (var index = 0; index < shellsPerShot; index++)
            {
                // Get random point inside unit circle
                var pointAtCircle = (Vector3) Random.insideUnitCircle;
                pointAtCircle.z = shellsDispersion;
                
                // Obtain world location with respect to shot start point position
                var localDirectionPoint = shotStartPoint.localPosition + pointAtCircle;
                var worldDirectionPoint = shotStartPoint.TransformPoint(localDirectionPoint);
                
                // Compute direction
                var direction = (worldDirectionPoint - shotStartPoint.position).normalized;
                
                if (!Physics.Raycast(shotStartPoint.position,
                    direction.normalized, out Hit, Range)) continue;
                Debug.DrawLine(shotStartPoint.position, Hit.point, Color.yellow, 3);
            
                if (Hit.transform.CompareTag("Player"))
                {
                    var playerLiving = Hit.transform.GetComponent<ILiving>();
                    playerLiving.Hurt(Damage);
                }

                var hitVFXInstance = Instantiate(hitVFX, Hit.point, Quaternion.identity);
                Destroy(hitVFXInstance, 1);
            }
        }

        // Draw shells dispersion
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            
            Gizmos.color = Color.green;

            for (var index = 0; index < 20; index++)
            {
                var angle = 360 / 20 * index;
                var pointOnCircle = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                var pointOnCircle3D = (Vector3) pointOnCircle;
                pointOnCircle3D.z = shellsDispersion;

                var localPoint = shotStartPoint.localPosition + pointOnCircle3D;
                var worldPoint = shotStartPoint.TransformPoint(localPoint);

                var direction = (worldPoint - shotStartPoint.position).normalized;
            
                Gizmos.DrawLine(shotStartPoint.position, shotStartPoint.position + direction * Range);
            }
        }
    }
}