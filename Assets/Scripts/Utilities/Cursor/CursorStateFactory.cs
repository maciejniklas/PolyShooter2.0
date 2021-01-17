using Patterns.Implementations;

namespace Utilities.Cursor
{
    public static class CursorStateFactory
    {
        public static State Accessible => new CursorAccessibleState();
        public static State Locked => new CursorLockState();
    }
}