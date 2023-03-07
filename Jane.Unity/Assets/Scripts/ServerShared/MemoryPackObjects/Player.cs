using System;
using MemoryPack;
using UnityEngine;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial class GamePlayerData
    {
        public Ulid GameId { get; set; }
        public string UserId { get; set; }
        public Ulid UniqueId { get; set; }
        public bool IsInitialized { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public int CurrentRegion { get; set; }
        public int CurrentZone { get; set; }
        public int HP { get; set; }
    }
}
