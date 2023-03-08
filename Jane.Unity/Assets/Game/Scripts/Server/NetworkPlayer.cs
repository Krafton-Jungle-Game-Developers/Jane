using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Jane.Unity;
using Jane.Unity.ServerShared.MemoryPackObjects;

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

    private SpaceshipEngine engine;
    public SpaceshipEngine Engine => engine;

    [SerializeField] private MeshRenderer modelRenderer;

    private void Awake()
    {
        if (_isSelf) { TryGetComponent(out engine); }
    }

    private async UniTaskVoid OnTriggerEnter(Collider other)
    {
        if (_isSelf is false) { return; }

        if (other.CompareTag("CheckPoint"))
        {
            CheckPoint checkPoint = other.GetComponent<CheckPoint>();
            CurrentRegion = checkPoint.Region;
            CurrentZone = checkPoint.CheckPointNumber;

            checkPoint.Interact();

            if (checkPoint.IsWarpGate)
            {
                engine.ControlsDisabled = true;
                await checkPoint.Warp(transform);
                engine.ControlsDisabled = false;
            }
        }
    }

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

    //private void Update()
    //{
    //    RankManager rm = RankManager.instance;
    //    if (!rm.checkPoints.goalActive)
    //    {
    //        distanceToCheckpoint = rm.GetDistance(this.gameObject, rm.checkPoints.checkPointArr[activeCheckpointIndex]);
    //    }
    //}

    public void UpdateMovement(MoveRequest request)
    {
        transform.SetPositionAndRotation(request.Position, request.Rotation);
    }

    public void OnLeaveRoom()
    {
        Destroy(gameObject);
    }
}
