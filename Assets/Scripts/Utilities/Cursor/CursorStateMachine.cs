using Patterns.Implementations;
using Patterns.Interfaces;

namespace Utilities.Cursor
{
    /// <summary>
    /// State machine for cursor accessibility
    /// </summary>
    public sealed class CursorStateMachine : StateMachine
    {
        private static CursorStateMachine Instance => _instance ?? (_instance = new CursorStateMachine());

        private static CursorStateMachine _instance;
        
        private CursorStateMachine() {}

        public new static void ChangeState(IState newState)
        {
            ((StateMachine) Instance).ChangeState(newState);
        }
    }
}