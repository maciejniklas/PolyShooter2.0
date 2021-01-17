using Characters.Player;
using Cinemachine;
using UnityEngine;

namespace Utilities
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private void Start()
        {
            virtualCamera.Follow = PlayerModule.LocalPlayer.transform;
            virtualCamera.LookAt = PlayerModule.LocalPlayer.transform;
        }
    }
}