using Patterns.Interfaces;

namespace Patterns.Implementations
{
    public abstract class StateMachine : IStateMachine
    {
        public IState CurrentState { get; private set; }
        
        protected StateMachine() {}

        protected StateMachine(IState initialState)
        {
            ChangeState(initialState);
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