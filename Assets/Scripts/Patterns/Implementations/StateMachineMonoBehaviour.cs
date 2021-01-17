using System;
using Patterns.Interfaces;
using UnityEngine;

namespace Patterns.Implementations
{
    [Serializable]
    public class StateMachineMonoBehaviour : MonoBehaviour, IStateMachine
    {
        [Tooltip("Initial state of state this state machine. Leave as null if there shouldn't be initial state.")]
        [SerializeField] private StateMonoBehaviour initialState;
        
        public IState CurrentState { get; private set; }

        private void Start()
        {
            if (initialState != null)
            {
                ChangeState(initialState);
            }
        }

        private void Update()
        {
            CurrentState?.UpdateState();
        }
        
        public void ChangeState(StateMonoBehaviour newState)
        {
            ChangeState(newState as IState);
        }
        
        public void ChangeState(IState newState)
        {
            CurrentState?.DestroyState();

            CurrentState = newState;

            if (CurrentState == null) return;
            CurrentState.Owner = this;
            CurrentState.PrepareState();
        }
    }
}