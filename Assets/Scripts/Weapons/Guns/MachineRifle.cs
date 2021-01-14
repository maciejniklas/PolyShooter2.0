using Characters.Interfaces;
using Photon.Pun;
using UI;
using UnityEngine;

namespace Weapons.Guns
{
    public class MachineRifle : Firearm
    {
        protected override void Update()
        {
            base.Update();

            if (!photonView.IsMine) return;
            if (Input.GetButton("Fire1")) Attack();
        }
        
        [PunRPC]
        public override void Attack()
        {
            if (photonView.IsMine)
            {
                if (!_isAbleToShoot) return;
                
                if (BulletsInMagazine <= 0)
                {
                    Notification.Instance.ErrorMessage("No more bullets!");
                    return;
                }

                BulletsInMagazine -= 1;
                
                photonView.RPC(nameof(Attack), RpcTarget.Others);

                StartCoroutine(FireRateCooldown());
            }

            if (!Physics.Raycast(shotStartPoint.position, shotStartPoint.forward, out hit, Range)) return;
            if (!hit.transform.CompareTag("Player")) return;
            var playerLiving = hit.transform.GetComponent<ILiving>();
            playerLiving.Hurt(Damage);
        }
    }
}