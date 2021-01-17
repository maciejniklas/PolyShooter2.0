namespace Patterns.Interfaces
{
    public interface IState
    {
        IStateMachine Owner { get; set; }

        void DestroyState();
        void PrepareState();
        void UpdateState();
    }
}