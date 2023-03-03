using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IGameHubReceiver
    {
        void OnJoin(GamePlayerData joinedPlayer);
        void OnLeave(GamePlayerData request);
        void OnMove(MoveRequest request);
    }
}
