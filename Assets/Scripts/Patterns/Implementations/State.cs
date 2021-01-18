using Patterns.Interfaces;

namespace Patterns.Implementations
{
    public abstract class State : IState
    {
        public IStateMachine Owner { get; set; }

        public abstract void DestroyState();

        public abstract void PrepareState();

        public virtual void UpdateState() { }
    }
}