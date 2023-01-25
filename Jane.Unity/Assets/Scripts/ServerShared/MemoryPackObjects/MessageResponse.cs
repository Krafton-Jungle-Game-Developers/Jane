using MemoryPack;

namespace Jane.Unity.ServerShared.MemoryPackObjects
{
    [MemoryPackable]
    public partial struct MessageResponse
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}