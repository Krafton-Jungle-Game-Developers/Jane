using System;
using Jane.Unity.ServerShared.Enums;

public class GameInfo
{
    public static Ulid GameId = Ulid.Empty;
    public static int PlayerCount = 0;
    public static GameState GameState = GameState.Waiting;
}