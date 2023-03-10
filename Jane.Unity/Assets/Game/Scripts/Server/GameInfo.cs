using System;
using Jane.Unity.ServerShared.Enums;

public static class GameInfo
{
    public static Ulid GameId = Ulid.Empty;
    public static int PlayerCount = 0;
    public static GameState GameState = GameState.Waiting;
    public static long GameTime = 0L;
}