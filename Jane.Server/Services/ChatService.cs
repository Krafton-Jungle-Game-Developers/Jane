using System;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.Extensions.Logging;

namespace Jane.Server.Services;

public class ChatService : ServiceBase<IChatService>, IChatService
{
    private readonly ILogger _logger;

    public ChatService(ILogger<ChatService> logger)
    {
        _logger = logger;
    }

    public UnaryResult GenerateException(string message)
    {
        throw new NotImplementedException();
    }

    public UnaryResult SendReportAsync(string message)
    {
        _logger.LogDebug($"{message}");

        return UnaryResult.CompletedResult;
    }
}
