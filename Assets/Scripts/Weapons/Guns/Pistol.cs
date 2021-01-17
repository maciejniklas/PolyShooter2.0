using Characters.Interfaces;
using Photon.Pun;
using UI;
using UnityEngine;

namespace Weapons.Guns
{
    public class Pistol : Firearm
    {
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

            if (!Physics.Raycast(shotStartPoint.position, shotStartPoint.forward, out Hit, Range)) return;
            if (!Hit.transform.CompareTag("Player")) return;
            var playerLiving = Hit.transform.GetComponent<ILiving>();
            playerLiving.Hurt(Damage);
        }
    }
}