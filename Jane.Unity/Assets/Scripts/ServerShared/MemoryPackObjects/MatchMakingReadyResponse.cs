using System;
using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct MatchMakingReadyResponse
    {
        public Ulid UniqueId { get; set; }
        public bool IsReady { get; set; }
    }
}