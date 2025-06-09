using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public static class LockManager
    {
        private static readonly ConcurrentDictionary<string, object> _locks = new();

        public static bool TryLock(string name)
        {
            return _locks.TryAdd(name,new object());
        }
        public static void Unlock(string name)
        {
            _locks.TryRemove(name, out _);
        }
        public static bool IsLocked(string name)
        {
            return _locks.ContainsKey(name);
        }

    }
}
