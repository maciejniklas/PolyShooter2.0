using System;
using Photon.Pun;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerModule : MonoBehaviourPun
    {
        [Header("Initialization")]
        [SerializeField] private GameObject virtualCamera;
        [SerializeField] private GameObject visualRepresentation;
        
        public static PlayerModule LocalPlayer { get; private set; }

        private void Awake()
        {
            if (!photonView.IsMine) return;
            
            // Singleton
            if (LocalPlayer != null)
            {
                Destroy(gameObject);
            }
            else
            {
                LocalPlayer = this;
            }
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                InitializeFpp();
            }
        }

        private void OnDestroy()
        {
            if (!photonView.IsMine) return;
            
            // Destroy singleton reference
            if (LocalPlayer == this)
            {
                LocalPlayer = null;
            }
        }

        private void InitializeFpp()
        {
            visualRepresentation.SetActive(false);
            virtualCamera.SetActive(true);
        }
    }
}