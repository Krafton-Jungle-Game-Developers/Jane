using System;
using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial class MatchMakingLobbyUser
    {
        public string UserId { get; set; }
        public Ulid UniqueId { get; set; }
        public bool IsReady { get; set; }
    }   
}