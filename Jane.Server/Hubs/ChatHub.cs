using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion.Server.Hubs;

namespace Jane.Server.Hubs;

public class ChatHub : StreamingHubBase<IChatHub, IChatHubReceiver>, IChatHub
{
    private IGroup _room;
    private string _myName;

    public async ValueTask JoinAsync(JoinRequest request)
    {
        _room = await Group.AddAsync(request.RoomName);
        _myName = request.UserName;

        Broadcast(_room).OnJoin(request.UserName);
    }

    public async ValueTask LeaveAsync()
    {
        await _room.RemoveAsync(this.Context);

        Broadcast(_room).OnLeave(_myName);
    }

    public async ValueTask SendMessageAsync(string message)
    {
        MessageResponse response = new() { UserName = _myName, Message = message };
        Broadcast(_room).OnSendMessage(response);

        await CompletedTask;
    }

    public ValueTask GenerateException(string message)
    {
        throw new Exception(message);
    }

    public ValueTask SampleMethod(List<int> sampleList, Dictionary<int, string> sampleDictionary)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask OnConnecting()
    {
        // Handle Connection if needed
        Console.WriteLine($"Client CONNECTED: {this.Context.ContextId}");
        return CompletedTask;
    }

    protected override ValueTask OnDisconnected()
    {
        // Handle disconnection if needed
        return CompletedTask;
    }
}
