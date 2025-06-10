using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Server.Services
{
    public static class LockManager
    {
        private static readonly ConcurrentDictionary<string, LockInfo> _locks = new();

        public static bool TryLock(string name)
        {
            var lockInfo = new LockInfo
            {
                AcquiredAt = DateTime.UtcNow,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };

            return _locks.TryAdd(name, lockInfo);
        }

        public static void Unlock(string name)
        {
            _locks.TryRemove(name, out _);
        }

        public static bool IsLocked(string name)
        {
            return _locks.ContainsKey(name);
        }



        private class LockInfo
        {
            public DateTime AcquiredAt { get; set; }
            public int ThreadId { get; set; }
        }
    }
}