using System;
using Jane.Unity;
using Jane.Unity.ServerShared.MemoryPackObjects;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    private string userId;
    public string UserId => userId;
    private Ulid uniqueId;
    public Ulid UniqueId => uniqueId;
    public int CurrentRegion { get; set ; } = 1;
    public int CurrentZone { get; set; } = 0;
    public int HP { get; set; } = 20;

    public int activeCheckpointIndex { get; set; }
    public float distanceToCheckpoint { get; set; }

    private bool _isSelf;
    public bool IsSelf => _isSelf;
    
    [SerializeField] private MeshRenderer modelRenderer;
    
    public void Initialize(GamePlayerData data, bool isSelf)
    {
        userId = data.UserId;
        uniqueId = data.UniqueId;
        CurrentRegion = data.CurrentRegion;
        CurrentZone = data.CurrentZone;
        HP = data.HP;

        transform.SetPositionAndRotation(data.Position, data.Rotation);
        modelRenderer.enabled = true;

        _isSelf = isSelf;
        gameObject.name = data.UserId;
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
