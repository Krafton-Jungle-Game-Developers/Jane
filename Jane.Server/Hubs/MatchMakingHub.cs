using System;
using MagicOnion.Server.Hubs;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Server.Hubs
{
    public class MatchMakingHub : StreamingHubBase<IMatchMakingHub, IMatchMakingHubReceiver>, IMatchMakingHub
    {
        private IGroup? matchMakingLobby = null;
        private MatchMakingLobbyUser? self = null;
        private IInMemoryStorage<MatchMakingLobbyUser>? storage;
        private bool isEnrollSuccessful = false;
        private bool isAllPlayersReady = false;
        private Ulid matchId = Ulid.Empty;
        private Ulid matchedGameId = Ulid.Empty;

        public async ValueTask<MatchMakingEnrollResponse> EnrollAsync(MatchMakingEnrollRequest request)
        {
            self = new () { UserId = request.UserId, UniqueId = request.UniqueId };

            // TODO: Get matchId from somewhere
            matchId = Ulid.MinValue;
            (isEnrollSuccessful, matchMakingLobby, storage) = await Group.TryAddAsync(matchId.ToString(), 4, true, self);

            if (isEnrollSuccessful is false) { return new () { MatchId = Ulid.Empty, LobbyUsers = null }; }

            BroadcastExceptSelf(matchMakingLobby).OnEnroll(self);
            MatchMakingEnrollResponse response = new () { MatchId = matchId, LobbyUsers = storage.AllValues.ToArray() };
            return response;
        }

        // TODO: 2명 이상? 
        public async ValueTask ChangeReadyStateAsync(MatchMakingReadyRequest request)
        {
            if (isAllPlayersReady) { return; }

            self.IsReady = request.IsReady;
            isAllPlayersReady = storage.AllValues.All(user => user.IsReady);
            MatchMakingReadyResponse res = new () { UniqueId = request.UniqueId, IsReady = self.IsReady };
            Broadcast(matchMakingLobby).OnPlayerReadyStateChanged(res);
            
            if (isAllPlayersReady)
            {
                matchedGameId = Ulid.NewUlid();
                MatchMakingCompleteResponse response = new() { GameId = matchedGameId, PlayerCount = await matchMakingLobby.GetMemberCountAsync() };
                Broadcast(matchMakingLobby).OnMatchMakingComplete(response);
                await matchMakingLobby.RemoveAsync(Context);
            }
        }
        
        public async ValueTask LeaveAsync()
        {
            Broadcast(matchMakingLobby).OnLeave(self);
            await matchMakingLobby.RemoveAsync(Context);
        }
        
        protected override ValueTask OnDisconnected()
        {
            BroadcastExceptSelf(matchMakingLobby).OnLeave(self);
            return CompletedTask;
        }
    }
}
