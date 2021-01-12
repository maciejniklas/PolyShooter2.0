using UnityEngine;

namespace Patterns
{
    /// <summary>
    /// Base stat implementation for state machine
    /// </summary>
    public abstract class BaseState : MonoBehaviour
    {
        /// <summary>
        /// Reference to owning state machine
        /// </summary>
        [HideInInspector]
        public StateMachine owner;
        
        /// <summary>
        /// Method called for state preparation to work
        /// </summary>
        public virtual void PrepareState() {}
        
        /// <summary>
        /// Method called for state update on every frame
        /// </summary>
        public virtual void UpdateState() {}
        
        /// <summary>
        /// Method called for state destroy
        /// </summary>
        public virtual void DestroyState() {}
    }
}