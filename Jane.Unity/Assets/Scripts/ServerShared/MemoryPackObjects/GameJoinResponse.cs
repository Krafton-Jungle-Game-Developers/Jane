using System;
using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct GameJoinResponse
    {
        public Ulid GameId { get; set; }
        public GamePlayerData[] Players { get; set; }
    }
}
