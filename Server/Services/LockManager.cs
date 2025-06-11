using System;
using System.Collections.Concurrent;
using System.Threading;
using Common.Errors;
using Common.Events;

namespace Server.Services
{
    public class LockManager
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _locks = new();

        public LockManager()
        {
            LockHub.LockRequested += HandleLock;
            LockHub.UnlockRequested += HandleUnlock;
        }

        private void HandleLock(string name)
        {
            TryLock(name, out _);
        }

        private void HandleUnlock(string name)
        {
            Unlock(name);
        }

        public bool TryLock(string name, out CancellationToken token)
        {
            var cts = new CancellationTokenSource();
            if (_locks.TryAdd(name, cts))
            {
                token = cts.Token;
                Console.WriteLine($"Locked: {name}");
                return true;
            }

            token = CancellationToken.None;
            Console.WriteLine(AppErrors.File.Locked);
            return false;
        }

        public void Unlock(string name)
        {
            if (!_locks.TryRemove(name, out var cts)) return;
            cts.Cancel();
            cts.Dispose();
            Console.WriteLine($"Unlocked: {name}");
        }

        public bool IsLocked(string name) => _locks.ContainsKey(name);
    }
}
