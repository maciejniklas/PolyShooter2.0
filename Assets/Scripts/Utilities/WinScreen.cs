using Characters.Player;
using Cinemachine;
using UnityEngine;

namespace Utilities
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        public void Initialize(Transform winnerTransform)
        {
            virtualCamera.Follow = winnerTransform;
            virtualCamera.LookAt = winnerTransform;
        }
    }
}