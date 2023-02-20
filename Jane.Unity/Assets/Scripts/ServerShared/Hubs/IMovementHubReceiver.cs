using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IMovementHubReceiver
    {
        void OnJoin(Player request);
        void OnLeave(Player request);
        void OnMove(MoveRequest request);
    }
}
