using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Server.Services
{
    public static class LockManager
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _locks = new();

        public static bool TryLock(string name,out CancellationToken token)
        {
            var cts = new CancellationTokenSource();
            if (_locks.TryAdd(name, cts))
            {
                token = cts.Token;
                return true;
            }

            token = CancellationToken.None;
            return false;
        }

        public static void Unlock(string name)
        {
            if (_locks.TryRemove(name, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }
        }
    }
}