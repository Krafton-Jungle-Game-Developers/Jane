using System;
using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IMatchMakingHubReceiver
    {
        void OnEnroll(MatchMakingLobbyUser user);
        void OnLeave(MatchMakingLobbyUser leftUser);
        void OnPlayerReadyStateChanged(Ulid uniqueId, bool isReady);
        void OnMatchMakingComplete(MatchMakingCompleteResponse response);
    }
}
