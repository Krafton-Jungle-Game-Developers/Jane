using System;
using Jane.Unity.ServerShared.MemoryPackObjects;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public string UserId { get; set; }
    public Ulid UniqueId { get; set; }
    public int activeCheckpointIndex { get; set; }
    public float distanceToCheckpoint { get; set; }
    public void Initialize(GamePlayerData data)
    {
        UserId = data.UserId;
        UniqueId = data.UniqueId;
    }

    public void UpdateMovement(MoveRequest request)
    {
        transform.SetPositionAndRotation(request.Position, request.Rotation);
    }

    public void OnLeaveRoom()
    {
        Destroy(gameObject);
    }
}
