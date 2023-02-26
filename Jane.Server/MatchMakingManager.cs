using System;
using System.Collections.Concurrent;
using MagicOnion.Server;

namespace Jane.Server
{
    public class MatchMakingManager
    {
        private readonly ConcurrentDictionary<Ulid, (Ulid, ServiceContext)> matchMakingEntries = new();

        public void TryMatching(Ulid connectionId, ServiceContext context)
        {
            if (matchMakingEntries.Count >= 1)
            {

            }
            else
            {
                matchMakingEntries.TryAdd()
            }
        }

        public void RemoveFromMatchMakingEntries(Ulid userId)
        {
            matchMakingEntries.TryRemove(userId, out _);
        }
    }
}
