using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IChatHubReceiver
    {
        void OnJoin(string userName);
        void OnLeave(string userName);
        void OnSendMessage(MessageResponse message);
    }
}
