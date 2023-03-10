using System;
using Jane.Unity.ServerShared.Enums;
using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct GameStateChangedResponse
    {
        public Ulid GameId { get; set; }
        public GameState GameState { get; set; }
    }
}