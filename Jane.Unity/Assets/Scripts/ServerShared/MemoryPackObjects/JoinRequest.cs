using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct JoinRequest
    {
        public string RoomName { get; set; }
        public string UserName { get; set; }
    }
}
