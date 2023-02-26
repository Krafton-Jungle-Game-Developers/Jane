using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IGameHubReceiver
    {
        void OnJoin(Player request);
        void OnLeave(Player request);
        void OnMove(MoveRequest request);
    }
}
