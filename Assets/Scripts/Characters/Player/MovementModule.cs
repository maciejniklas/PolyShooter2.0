using System;
using Photon.Pun;
using UnityEngine;
using Utilities;

namespace Characters.Player
{
    /// <summary>
    /// Responsible for handling with user local input
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class MovementModule : MonoBehaviourPun
    {
        [Header("General")]
        [SerializeField] private float speed = 4;
        [SerializeField] private float jumpForce = 4;

        [Header("Ground detection")]
        [SerializeField] private TriggerSensor groundSensor;
        
        private Vector2 _userInput;
        private Vector3 _movementDirection;
        private Rigidbody _rigidbody;
        private bool _isGrounded;
        
        private void Awake()
        {
            // Remove module if this is not local client of this player instance
            if (!photonView.IsMine)
            {
                Destroy(this);
            }
            
            // Initialization
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            groundSensor.gameObject.SetActive(true);
        }

        private void Update()
        {
            // Grab user input
            if (_isGrounded)
            {
                _userInput.x = Input.GetAxis("Horizontal");
                _userInput.y = Input.GetAxis("Vertical");
            }
            
            // Compute the direction
            _movementDirection = (transform.right * _userInput.x + transform.forward * _userInput.y).normalized;
            
            // Detect jump
            if (Input.GetButtonDown("Jump"))
            {
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
        }

        private void FixedUpdate()
        {
            // Move
            _rigidbody.MovePosition(transform.position + _movementDirection * (Time.deltaTime * speed));
        }

        private void OnDisable()
        {
            groundSensor.OnTriggerChangeDetection -= UpdateGroundedInfo;
        }

        private void OnEnable()
        {
            groundSensor.OnTriggerChangeDetection += UpdateGroundedInfo;
        }

        private void UpdateGroundedInfo(bool isInside)
        {
            _isGrounded = isInside;
        }
    }
}