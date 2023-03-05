using System;
using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct MatchMakingEnrollRequest
    {
        public string UserId { get; set; }
        public Ulid UniqueId { get; set; }
    }
}
