namespace Patterns.Interfaces
{
    public interface IStateMachine
    {
        IState CurrentState { get; }

        void ChangeState(IState newState);
    }
}