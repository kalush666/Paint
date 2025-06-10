namespace Server.Events
{
    public static class LockHub
    {
        public static event Action<string>? LockRequested;
        public static event Action<string>? UnlockRequested;

        public static void TriggerLock(string name)
        {
            LockRequested?.Invoke(name);
        }

        public static void TriggerUnlock(string name)
        {
            UnlockRequested?.Invoke(name);
        }
    }
}