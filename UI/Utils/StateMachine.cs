namespace UI.Utils
{
    public static class StateMachine
    {
        private static object _owner;
        public static ProcessStates CurrentState { get; private set; } = ProcessStates.Idle;

        public static bool TrySetState(ProcessStates state, object sender)
        {
            if (_owner != null && _owner != sender) return false;
            _owner = sender;
            CurrentState = state;
            return true;
        }
    }
}