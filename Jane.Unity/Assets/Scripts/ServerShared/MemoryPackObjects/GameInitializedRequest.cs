using System;
using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct GameInitializedRequest
    {
        public Ulid UniqueId { get; set; }
    }
}