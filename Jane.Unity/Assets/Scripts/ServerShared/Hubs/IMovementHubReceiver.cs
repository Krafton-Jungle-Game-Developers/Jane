using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IMovementHubReceiver
    {
        void OnJoin(MoveRequest request);
        void OnLeave(MoveRequest request);
        void OnMove(MoveRequest request);
    }
}
