using Photon.Pun;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerModule : MonoBehaviourPun
    {
        [Header("Initialization")]
        [SerializeField] private GameObject virtualCamera;
        [SerializeField] private GameObject visualRepresentation;

        private void Start()
        {
            if (photonView.IsMine)
            {
                InitializeFpp();
            }
        }

        private void InitializeFpp()
        {
            visualRepresentation.SetActive(false);
            virtualCamera.SetActive(true);
        }
    }
}