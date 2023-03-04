using System;
using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct MatchMakingCompleteResponse
    {
        public Ulid GameId { get; set; }
        public int PlayerCount { get; set; }
    }
}
