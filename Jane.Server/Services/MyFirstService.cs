using Jane.Shared;

namespace Jane.Server.Services;

public class MyFirstService : IService<IMyFirstService>
{
    public async UnaryResult<int> SumAsync(int x, int y)
    {
        return await UnaryResult.FromResult(x + y);
    }
}
