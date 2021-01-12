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
        [SerializeField] private float speed = 3;
        
        private Vector2 _userInput;
        private Vector3 _movementDirection;
        private Rigidbody _rigidbody;
        
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

        private void Update()
        {
            // Grab user input
            _userInput.x = Input.GetAxis("Horizontal");
            _userInput.y = Input.GetAxis("Vertical");
            
            // Compute the direction
            _movementDirection = (transform.right * _userInput.x + transform.forward * _userInput.y).normalized;
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(transform.position + _movementDirection * (Time.deltaTime * speed));
        }
    }
}