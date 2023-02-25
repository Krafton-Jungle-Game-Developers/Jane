using System;
using MagicOnion.Server.Hubs;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion.Server;

namespace Jane.Server.Hubs
{
    public class MatchMakingHub : StreamingHubBase<IMatchMakingHub, IMatchMakingHubReceiver>, IMatchMakingHub
    {
        private IGroup matchMakingRoom;
        private MatchMakingManager _matchMakingManager;
        private Ulid connectionId;

        public MatchMakingHub(MatchMakingManager manager)
        {
            _matchMakingManager = manager;
            connectionId = new Ulid(ConnectionId);
        }

        public async ValueTask JoinAsync(MatchMakingEnrollRequest request)
        {
            matchMakingRoom = await Group.AddAsync("MatchMakingRoom");

            _matchMakingManager.TryMatching(connectionId, Context);
            
        }

        public async ValueTask LeaveAsync()
        {
            throw new NotImplementedException();
        }

        protected override ValueTask OnDisconnected()
        {
            _matchMakingManager.RemoveFromMatchMakingEntries();
        }
    }
}
