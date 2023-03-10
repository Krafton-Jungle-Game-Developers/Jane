using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IGameHubReceiver
    {
        void OnJoin(GamePlayerData joinedPlayerData);
        void OnLeave(GamePlayerData request);
        void OnGameStateChange(GameStateChangedResponse response);
        void OnTimerUpdate(long ticks);
        void OnMove(MoveRequest request);
    }
}
