using MagicOnion;
using Jane.Unity.ServerShared.MemoryPackObjects;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IChatHub : IStreamingHub<IChatHub, IChatHubReceiver>
    {
        ValueTask JoinAsync(JoinRequest request);
        ValueTask LeaveAsync();
        ValueTask SendMessageAsync(string message);
        ValueTask GenerateException(string message);
        ValueTask SampleMethod(List<int> sampleList, Dictionary<int, string> sampleDictionary);
    }
}
