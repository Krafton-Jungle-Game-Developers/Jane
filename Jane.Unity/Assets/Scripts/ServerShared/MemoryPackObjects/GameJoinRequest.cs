using System;
using MemoryPack;
using UnityEngine;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct GameJoinRequest
    {
        public Ulid GameId { get; set; }
        public int PlayerCount { get; set; }
        public string UserId { get; set; }
        public Ulid UniqueId { get; set; }
        public Vector3 InitialPosition { get; set; } 
        public Quaternion InitialRotation { get; set; }
    }
}