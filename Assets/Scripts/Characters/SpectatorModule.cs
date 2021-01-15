using System;
using UnityEngine;

namespace Characters
{
    public class SpectatorModule : MonoBehaviour
    {
        [SerializeField] private float speed = 10;
        [SerializeField] private float mouseSensitivity = 150f;
        
        private Vector2 _userInput;
        private Vector3 _movementDirection;
        private Vector2 _mouseInput;

        private float _pitch;
        private float _yaw;
        private Vector3 _upOrDownDirection;

        private void Awake()
        {
            _pitch = 0f;
            _yaw = 0f;
        }

        private void Update()
        {
            _userInput.x = Input.GetAxis("Horizontal");
            _userInput.y = Input.GetAxis("Vertical");

            if (Input.GetButton("ReleaseCursor"))
            {
                _mouseInput = Vector2.zero;
            }
            else
            {
                _mouseInput.x = Input.GetAxis("Mouse X");
                _mouseInput.y = Input.GetAxis("Mouse Y");
            }

            if (Input.GetKey(KeyCode.E))
            {
                _upOrDownDirection = Vector3.up;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                _upOrDownDirection = Vector3.down;
            }
            else
            {
                _upOrDownDirection = Vector3.zero;;
            }

            _movementDirection = (transform.right * _userInput.x + transform.forward * _userInput.y + _upOrDownDirection).normalized;
            _movementDirection *= speed * Time.deltaTime;

            _yaw += _mouseInput.x * mouseSensitivity * Time.deltaTime;
            _pitch -= _mouseInput.y * mouseSensitivity * Time.deltaTime;

            transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
        }

        private void FixedUpdate()
        {
            transform.Translate(_movementDirection, Space.World);
        }
    }
}