using UnityEngine;

namespace Patterns
{
    /// <summary>
    /// State machine implementation
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        /// <summary>
        /// Reference to initial state if there should be one
        /// </summary>
        [Tooltip("Initial state of state this state machine. Leave as null if there shouldn't be initial state.")]
        [SerializeField] private BaseState initialState;
        
        /// <summary>
        /// Reference to currently operating state
        /// </summary>
        private BaseState _currentStat;

        private void Start()
        {
            if (initialState != null)
            {
                ChangeState(initialState);
            }
        }

        private void Update()
        {
            if (!(_currentStat is null)) _currentStat.UpdateState();
        }

        /// <summary>
        /// Method used to change state
        /// </summary>
        /// <param name="newState">New BaseState used by the state machine</param>
        public void ChangeState(BaseState newState)
        {
            if (!(_currentStat is null)) _currentStat.DestroyState();

            _currentStat = newState;

            if (_currentStat == null) return;
            _currentStat.owner = this;
            _currentStat.PrepareState();
        }
    }
}