using MagicOnion;

namespace Jane.Unity.ServerShared.Services
{
    public interface IChatService : IService<IChatService>
    {
        UnaryResult GenerateException(string message);
        UnaryResult SendReportAsync(string message);
    }
}
