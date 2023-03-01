using System;
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
    [SerializeField] private LobbyPlayer myLobbyPlayerUI;
    [SerializeField] private LobbyPlayer[] otherLobbyPlayers = new LobbyPlayer[3];
    [SerializeField] private MatchMakingLobbyUser[] lobbyUsers;
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
            lobbyUsers = await _matchMakingHub.EnrollAsync(request);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }

        MatchMakingLobbyUser self = lobbyUsers.FirstOrDefault(user => user.UniqueId.Equals(UserInfo.UniqueId));
        myLobbyPlayerUI.SetPlayerName(self.UserId);
        myLobbyPlayerUI.SetAdditionalText(self.UniqueId.ToString());
        myLobbyPlayerUI.SetState(LobbyPlayer.ItemState.NotReady);

        for (int i = 0; i < lobbyUsers.Length; i++)
        {
            if (lobbyUsers[i].UniqueId.Equals(self.UniqueId)) { continue; }

            otherLobbyPlayers[i].SetPlayerName(lobbyUsers[i].UserId);
            otherLobbyPlayers[i].SetAdditionalText(lobbyUsers[i].UniqueId.ToString());
            otherLobbyPlayers[i].SetState(LobbyPlayer.ItemState.NotReady);
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
        var emptySlot = lobbyUsers.FirstOrDefault(user => user is null || user.UniqueId.Equals(Ulid.Empty));
        if (emptySlot is null) { emptySlot = new(); }

        emptySlot.UserId = user.UserId;
        emptySlot.UniqueId = user.UniqueId;
        emptySlot.IsReady = user.IsReady;

        var emptyUISlot = otherLobbyPlayers.FirstOrDefault(user => user.currentState is LobbyPlayer.ItemState.Empty);
        emptyUISlot.SetPlayerName(user.UserId);
        emptyUISlot.SetAdditionalText(user.UniqueId.ToString());
        emptyUISlot.SetState(LobbyPlayer.ItemState.NotReady);
    }

    public void OnLeave(MatchMakingLobbyUser leftUser)
    {
        // TODO: me
        if (leftUser.UniqueId.Equals(UserInfo.UniqueId))
        {
            lobbyUsers = null;

            myLobbyPlayerUI.SetPlayerName(string.Empty);
            myLobbyPlayerUI.SetAdditionalText(string.Empty);
            myLobbyPlayerUI.SetState(LobbyPlayer.ItemState.Empty);

            foreach (var otherLobbyPlayer in otherLobbyPlayers)
            {
                otherLobbyPlayer.SetPlayerName(string.Empty);
                otherLobbyPlayer.SetAdditionalText(string.Empty);
                otherLobbyPlayer.SetState(LobbyPlayer.ItemState.Empty);
            }
        }
        else
        {
            var otherUser = lobbyUsers.FirstOrDefault(user => user.UniqueId.Equals(leftUser.UniqueId));
            otherUser = null;

            var otherUserUI = otherLobbyPlayers.FirstOrDefault(user => user.additionalText.Equals(leftUser.UniqueId.ToString()));
            otherUserUI.SetPlayerName(string.Empty);
            otherUserUI.SetAdditionalText(string.Empty);
            otherUserUI.SetState(LobbyPlayer.ItemState.Empty);
        }
    }

    public void OnPlayerReadyStateChanged(Ulid uniqueId, bool isReady)
    {
        lobbyUsers.First(user => user.UniqueId.Equals(uniqueId)).IsReady = isReady;
        otherLobbyPlayers.First(user => user.additionalText.Equals(uniqueId.ToString()))
                         .SetState(isReady ? LobbyPlayer.ItemState.Ready 
                                           : LobbyPlayer.ItemState.NotReady);
    }

    public void OnMatchMakingComplete(MatchMakingCompleteResponse response)
    {
        Debug.Log($"MatchMake Complete! GameID:{response.GameId}");
    }
}
