using System;
public static class UserInfo
{
    public static readonly Ulid UniqueId = Ulid.NewUlid();
    public static readonly string UserId = UniqueId.ToString();
}