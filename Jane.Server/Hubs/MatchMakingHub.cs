using System;
using MagicOnion.Server.Hubs;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Server.Hubs
{
    public class MatchMakingHub : StreamingHubBase<IMatchMakingHub, IMatchMakingHubReceiver>, IMatchMakingHub
    {
        private IGroup? matchMakingLobby;
        private MatchMakingLobbyUser self;
        private IInMemoryStorage<MatchMakingLobbyUser>? storage;
        private bool isEnrollSuccessful;
        private bool isAllPlayersReady;
        private Ulid matchId;
        private Ulid matchedGameId;

        public async ValueTask<MatchMakingEnrollResponse> EnrollAsync(MatchMakingEnrollRequest request)
        {
            self = new () { UserId = request.UserId, UniqueId = request.UniqueId };
            
            (isEnrollSuccessful, matchMakingLobby, storage) = await Group.TryAddAsync("MatchMakingLobby", 4, true, self);

            if (isEnrollSuccessful is false) { return new () { MatchId = Ulid.Empty, LobbyUsers = null }; }

            BroadcastExceptSelf(matchMakingLobby).OnEnroll(self);
            MatchMakingEnrollResponse response = new () { MatchId = Ulid.MinValue, LobbyUsers = storage.AllValues.ToArray() };
            return response;
        }

        // TODO: 2명 이상? 
        public ValueTask ChangeReadyStateAsync(bool isReady)
        {
            if (isAllPlayersReady) { return CompletedTask; }

            self.IsReady = isReady;
            isAllPlayersReady = storage.AllValues.All(user => user.IsReady);
            Broadcast(matchMakingLobby).OnPlayerReadyStateChanged(self.UniqueId, isReady);
            
            if (isAllPlayersReady)
            {
                matchedGameId = Ulid.NewUlid();
                MatchMakingCompleteResponse response = new() { GameId = matchedGameId, PlayerCount = storage.AllValues.Count };
                Broadcast(matchMakingLobby).OnMatchMakingComplete(response);
            }

            return CompletedTask;
        }
        
        public async ValueTask LeaveAsync()
        {
            await matchMakingLobby.RemoveAsync(Context);
            Broadcast(matchMakingLobby).OnLeave(self);
        }
        
        protected override ValueTask OnDisconnected()
        {
            BroadcastExceptSelf(matchMakingLobby).OnLeave(self);
            return CompletedTask;
        }
    }
}
