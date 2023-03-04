using System;
using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct MatchMakingEnrollResponse
    {
        public Ulid MatchId { get; set; }
        public MatchMakingLobbyUser[] LobbyUsers { get; set; }
    }
}