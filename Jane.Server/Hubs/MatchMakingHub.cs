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
        private bool isEnrolled;
        private bool isAllPlayersReady;
        private Ulid gameId;

        public async ValueTask<MatchMakingLobbyUser[]?> EnrollAsync(MatchMakingEnrollRequest request)
        {
            self = new MatchMakingLobbyUser() { UserId = request.UserId, UniqueId = request.UniqueId };
            (isEnrolled, matchMakingLobby, storage) = await Group.TryAddAsync("MatchMakingLobby", 4, true, self);

            if (isEnrolled)
            {
                BroadcastExceptSelf(matchMakingLobby).OnEnroll(self);
                return storage.AllValues.ToArray();
            }
            
            return null;
        }

        // TODO: 2명 이상? 
        public async ValueTask ChangeReadyStateAsync(bool isReady)
        {
            if (isAllPlayersReady) { return; }
            self.IsReady = isReady;
            Broadcast(matchMakingLobby).OnPlayerReadyStateChanged(self.UniqueId, isReady);

            isAllPlayersReady = storage.AllValues.All(user => user.IsReady);

            if (isAllPlayersReady)
            {
                gameId = Ulid.NewUlid();
                MatchMakingCompleteResponse response = new() { GameId = gameId };
                Broadcast(matchMakingLobby).OnMatchMakingComplete(response);
            }
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
