using Photon.Pun;
using UnityEngine;

namespace Characters.Player
{
    /// <summary>
    /// Responsible for camera movement at player local client
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraMouseControlModule : MonoBehaviourPun
    {
        [SerializeField] private float mouseSensitivity = 150f;
        [SerializeField] private float minimalXRotationAngle = -60;
        [SerializeField] private float maximalXRotationAngle = 60;

        private Vector2 _mouseInput;
        private Vector2 _cameraDisplacement;
        private float _xRotation;

        private void Awake()
        {
            if (!photonView.IsMine) Destroy(this);
            
            _xRotation = 0f;
        }

        private void Update()
        {
            // Dont' move when cursor is visible
            if(Cursor.visible) return;
            
            // Grab mouse input
            _mouseInput.x = Input.GetAxis("Mouse X");
            _mouseInput.y = Input.GetAxis("Mouse Y");
            
            // Compute frame displacement
            _cameraDisplacement = _mouseInput * (Time.deltaTime * mouseSensitivity);

            // Handle rotation around X axis
            _xRotation -= _cameraDisplacement.y;
            _xRotation = Mathf.Clamp(_xRotation, minimalXRotationAngle, maximalXRotationAngle);
            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            // Handle rotation around Y axis
            PlayerModule.LocalPlayer.transform.Rotate(Vector3.up * _cameraDisplacement.x);
        }
    }
}