using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnityEngine;
using Michsky.UI.Reach;
using TMPro;

using Grpc.Core;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion;
using MagicOnion.Client;
using Cysharp.Threading.Tasks;

public class MatchMakingManager : MonoBehaviour, IMatchMakingHubReceiver
{
    private readonly CancellationTokenSource shutdownCts = new();
    private ChannelBase _channel;
    private IMatchMakingHub _matchMakingHub;

    private bool isJoin;
    private bool isSelfDisconnected;

    [SerializeField] private TMP_Text profileText;
    [SerializeField] private ButtonManager lobbyPlayButton;
    [SerializeField] private ButtonManager lobbyStopSearchButton;
    [SerializeField] private TMP_Text lobbyPlayerCountText;
    [SerializeField] private List<MatchMakingLobbyUser> lobbyUsers;
    [SerializeField] private LobbyPlayer[] lobbyUserUIPanels;
    [SerializeField] private HotkeyEvent readyInputEvent;
    [SerializeField] private HotkeyEvent unReadyInputEvent;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        lobbyPlayButton.onClick.AddListener(UniTask.UnityAction(async () => { await EnrollAsync(); }));
        lobbyStopSearchButton.onClick.AddListener(UniTask.UnityAction(async () => { await LeaveAsync(); }));
        readyInputEvent.onHotkeyPress.AddListener(UniTask.UnityAction(async () => { await ChangeReadyStateAsync(true); }));
        unReadyInputEvent.onHotkeyPress.AddListener(UniTask.UnityAction(async () => { await ChangeReadyStateAsync(false); }));
    }

    private async UniTaskVoid OnDestroy()
    {
        lobbyPlayButton.onClick.RemoveAllListeners();
        lobbyStopSearchButton.onClick.RemoveAllListeners();
        readyInputEvent.onHotkeyPress.RemoveAllListeners();
        unReadyInputEvent.onHotkeyPress.RemoveAllListeners();
        
        shutdownCts.Cancel();

        if (_matchMakingHub != null) { await _matchMakingHub.DisposeAsync(); }
        if (_channel != null) { await _channel.ShutdownAsync(); }
    }
    
    private async void Start()
    {
        await InitializeClientAsync();
        
        lobbyPlayerCountText.text = "1/4";
    }

    private async UniTask InitializeClientAsync()
    {
        // Initialize the Hub
        _channel = GrpcChannelx.ForTarget(new("jane.jungle-gamedev.com", 5001, false));

        while (!shutdownCts.IsCancellationRequested)
        {
            try
            {
                Debug.Log($"Connecting to the server...");
                _matchMakingHub = await StreamingHubClient.ConnectAsync<IMatchMakingHub, IMatchMakingHubReceiver>(_channel, this, cancellationToken: shutdownCts.Token).AsUniTask();
                Debug.Log($"Connection is established.");
                profileText.text = UserInfo.UserId;
                break;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            Debug.Log($"Failed to connect to the server. Retry after 5 seconds...");
            await UniTask.Delay(5 * 1000);
        }
    }

    private async UniTask EnrollAsync()
    {
        MatchMakingEnrollRequest request = new() { UserId = UserInfo.UserId, UniqueId = UserInfo.UniqueId };
        try
        {
            lobbyUsers = new(await _matchMakingHub.EnrollAsync(request));
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }

        for (int i = 0; i < lobbyUsers.Count; i++)
        {
            lobbyUserUIPanels[i].SetPlayerName(lobbyUsers[i].UserId);
            lobbyUserUIPanels[i].SetAdditionalText(lobbyUsers[i].UniqueId.ToString());
            lobbyUserUIPanels[i].SetState(LobbyPlayer.ItemState.NotReady);
        }
    }

    private async UniTask ChangeReadyStateAsync(bool isReady)
    {
        try
        {
            await _matchMakingHub.ChangeReadyStateAsync(isReady);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    // OnLeave will confirm server side Leave
    private async UniTask LeaveAsync()
    {
        try
        {
            await _matchMakingHub.LeaveAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }
    
    public void OnEnroll(MatchMakingLobbyUser user)
    {
        if (lobbyUsers.Any(lobbyUser => lobbyUser.UniqueId.Equals(user.UniqueId))) { return; }
        lobbyUsers.Add(user);

        var emptySlot = lobbyUserUIPanels.FirstOrDefault(panel => panel.currentState is LobbyPlayer.ItemState.Empty);

        emptySlot?.SetPlayerName(user.UserId);
        emptySlot?.SetAdditionalText(user.UniqueId.ToString());
        emptySlot?.SetState(LobbyPlayer.ItemState.NotReady);
    }

    public void OnLeave(MatchMakingLobbyUser leftUser)
    {
        if (leftUser.UniqueId.Equals(UserInfo.UniqueId))
        {
            lobbyUsers = null;
            foreach (var panel in lobbyUserUIPanels)
            {
                panel.SetPlayerName(string.Empty);
                panel.SetAdditionalText(string.Empty);
                panel.SetState(LobbyPlayer.ItemState.Empty);
            }
        }
        else
        {
            var otherUser = lobbyUsers.FirstOrDefault(user => user.UniqueId.Equals(leftUser.UniqueId));
            lobbyUsers.Remove(otherUser);

            var otherUserUI = lobbyUserUIPanels.FirstOrDefault(user => user.additionalText.Equals(leftUser.UniqueId.ToString()));
            otherUserUI?.SetPlayerName(string.Empty);
            otherUserUI?.SetAdditionalText(string.Empty);
            otherUserUI?.SetState(LobbyPlayer.ItemState.Empty);
        }
    }

    public void OnPlayerReadyStateChanged(Ulid uniqueId, bool isReady)
    {
        var lobbyUser = lobbyUsers.FirstOrDefault(user => user.UniqueId.Equals(uniqueId));
        if (lobbyUser is not null) { lobbyUser.IsReady = isReady; }

        var lobbyUserUI = lobbyUserUIPanels.FirstOrDefault(user => user.additionalText.Equals(uniqueId.ToString()));
        if (lobbyUserUI is not null) { lobbyUserUI.SetState(isReady ? LobbyPlayer.ItemState.Ready : LobbyPlayer.ItemState.NotReady); }
    }

    public void OnMatchMakingComplete(MatchMakingCompleteResponse response)
    {
        Debug.Log($"MatchMake Complete! GameID:{response.GameId}");
        // TODO: Fade
        // TODO: Load Game Scene
    }
}
