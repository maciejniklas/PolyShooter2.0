using Photon.Pun;
using UnityEngine;

namespace Utilities
{
    // Position point in some distance from the player with respect to local camera rotation
    public class RemoteWeaponLookAtPoint : MonoBehaviourPun
    {
        [SerializeField] private float range = 20f;
        [SerializeField] private Transform followingCameraTransform;

        private void Awake()
        {
            if (!photonView.IsMine) Destroy(this);
        }

        private void Update()
        {
            transform.position = followingCameraTransform.transform.position + followingCameraTransform.forward * range;
        }
    }
}