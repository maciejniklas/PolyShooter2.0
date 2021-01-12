using Photon.Pun;
using UnityEngine;

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
        [SerializeField] private float sprintSpeedValueBoost = 2f;
        [SerializeField] private float jumpForce = 4;

        [Header("Ground detection")]
        [SerializeField] private Transform groundSensor;
        [SerializeField] private float groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask groundLayer;
        
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
            
            // Detect sprint
            if (Input.GetButtonDown("Sprint"))
            {
                speed += sprintSpeedValueBoost;
            }
            else if (Input.GetButtonUp("Sprint"))
            {
                speed -= sprintSpeedValueBoost;
            }
        }

        private void FixedUpdate()
        {
            // Detect ground
            _isGrounded = Physics.CheckSphere(groundSensor.position, groundCheckDistance, groundLayer);
            
            // Move
            _rigidbody.MovePosition(transform.position + _movementDirection * (Time.deltaTime * speed));
        }

        private void UpdateGroundedInfo(bool isInside)
        {
            _isGrounded = isInside;
        }
    }
}