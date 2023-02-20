using System;
using MemoryPack;
using UnityEngine;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial class Player
    {
        public Ulid Id { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}
