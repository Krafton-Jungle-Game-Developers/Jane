using Grpc.Core;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using Jane.Unity.ServerShared.Services;
using MagicOnion;
using MagicOnion.Client;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

namespace Jane.Unity
{
    public class ChatComponent : MonoBehaviour, IChatHubReceiver
    {
        private readonly CancellationTokenSource _shutdownCts = new();
        private ChannelBase _channel;
        
        private IChatHub _streamingClient;
        private IChatService _client;

        private bool isJoin;
        private bool isSelfDisconnected;

        [SerializeField] private TMP_Text chatText;
        [SerializeField] private Button joinOrLeaveButton;
        [SerializeField] private TMP_Text joinOrLeaveButtonText;
        
        [SerializeField] private TMP_InputField messageInputField;
        [SerializeField] private Button sendMessageButton;

        [SerializeField] private Button reconnectButton;

        [SerializeField] private TMP_InputField reportInputField;
        [SerializeField] private Button sendReportButton;
        
        [SerializeField] private Button disconnectButton;
        [SerializeField] private Button exceptionButton;
        [SerializeField] private Button unaryExceptionButton;
        

        private async UniTaskVoid Start()
        {
            await
            (
                joinOrLeaveButton.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async (_) => await JoinOrLeaveAsync(), _shutdownCts.Token),
                sendMessageButton.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async (_) => await SendMessageAsync(), _shutdownCts.Token),
                reconnectButton.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async (_) => await ReconnectInitializedServerAsync(), _shutdownCts.Token),
                disconnectButton.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async (_) => await DisconnectServerAsync(), _shutdownCts.Token),
                unaryExceptionButton.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async (_) => await UnaryGenerateExceptionAsync(), _shutdownCts.Token),
                exceptionButton.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async (_) => await GenerateExceptionAsync(), _shutdownCts.Token),
                sendReportButton.OnClickAsAsyncEnumerable().ForEachAwaitAsync(async (_) => await SendReportAsync(), _shutdownCts.Token)
            );

            await InitializeClientAsync();
            InitializeUi();
        }

        private async UniTaskVoid OnDestroy()
        {
            // Clean up Hub and channel
            _shutdownCts.Cancel();

            if (_streamingClient != null) { await _streamingClient.DisposeAsync(); }
            if (_channel != null) { await _channel.ShutdownAsync(); }
        }

        private async UniTask InitializeClientAsync()
        {
            // Initialize the Hub
            _channel = GrpcChannelx.ForTarget(new("jane.jungle-gamedev.com", 5001, false));
            
            while (!_shutdownCts.IsCancellationRequested)
            {
                try
                {
                    Debug.Log($"Connecting to the server...");
                    _streamingClient = await StreamingHubClient.ConnectAsync<IChatHub, IChatHubReceiver>(_channel, this, cancellationToken: _shutdownCts.Token).AsUniTask();
                    RegisterDisconnectEvent(_streamingClient).Forget();
                    Debug.Log($"Connection is established.");
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                Debug.Log($"Failed to connect to the server. Retry after 5 seconds...");
                await UniTask.Delay(5 * 1000);
            }

            _client = MagicOnionClient.Create<IChatService>(_channel);
        }

        private void InitializeUi()
        {
            isJoin = false;

            sendMessageButton.interactable = false;
            chatText.text = string.Empty;
            messageInputField.text = string.Empty;
            messageInputField.placeholder.GetComponent<TMP_Text>().text = "Please enter your name.";
            joinOrLeaveButtonText.text = "Enter the room";
            exceptionButton.interactable = false;
        }

        private async UniTask RegisterDisconnectEvent(IChatHub streamingClient)
        {
            try
            {
                // you can wait disconnected event
                await streamingClient.WaitForDisconnect().AsUniTask();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                // try-to-reconnect? logging event? close? etc...
                Debug.Log($"Disconnected from the server.");

                if (isSelfDisconnected)
                {
                    // there is no particular meaning
                    await UniTask.Delay(2000);

                    // reconnect
                    await ReconnectServerAsync();
                }
            }
        }
        
        private async UniTask DisconnectServerAsync()
        {
            isSelfDisconnected = true;

            joinOrLeaveButton.interactable = false;
            sendMessageButton.interactable = false;
            sendReportButton.interactable = false;
            disconnectButton.interactable = false;
            exceptionButton.interactable = false;
            unaryExceptionButton.interactable = false;

            if (isJoin) { await JoinOrLeaveAsync(); }

            await _streamingClient.DisposeAsync().AsUniTask();
        }
        
        private async UniTask ReconnectInitializedServerAsync()
        {
            if (_channel != null)
            {
                var chan = _channel;
                if (chan == Interlocked.CompareExchange(ref _channel, null, chan))
                {
                    await chan.ShutdownAsync().AsUniTask();
                    _channel = null;
                }
            }
            
            if (_streamingClient != null)
            {
                var streamClient = _streamingClient;
                if (streamClient == Interlocked.CompareExchange(ref _streamingClient, null, streamClient))
                {
                    await streamClient.DisposeAsync().AsUniTask();
                    _streamingClient = null;
                }
            }

            if (_channel == null && _streamingClient == null)
            {
                await InitializeClientAsync();
                InitializeUi();
            }
        }

        private async UniTask ReconnectServerAsync()
        {
            Debug.Log($"Reconnecting to the server...");
            _streamingClient = await StreamingHubClient.ConnectAsync<IChatHub, IChatHubReceiver>(_channel, this).AsUniTask();
            RegisterDisconnectEvent(_streamingClient).Forget();
            Debug.Log("Reconnected.");

            joinOrLeaveButton.interactable = true;
            sendMessageButton.interactable = false;
            sendReportButton.interactable = true;
            disconnectButton.interactable = true;
            exceptionButton.interactable = true;
            unaryExceptionButton.interactable = true;

            isSelfDisconnected = false;
        }

        #region Client -> Server (Streaming)
        private async UniTask JoinOrLeaveAsync()
        {
            if (isJoin)
            {
                await _streamingClient.LeaveAsync();

                InitializeUi();
            }
            else
            {
                var request = new JoinRequest { RoomName = "SampleRoom", UserName = messageInputField.text };
                await _streamingClient.JoinAsync(request);

                isJoin = true;
                sendMessageButton.interactable = true;
                joinOrLeaveButtonText.text = "Leave the room";
                messageInputField.text = string.Empty;
                messageInputField.placeholder.GetComponent<TMP_Text>().text = "Please enter a comment.";
                exceptionButton.interactable = true;
            }
        }
        
        private async UniTask SendMessageAsync()
        {
            if (!isJoin) { return; }

            await _streamingClient.SendMessageAsync(messageInputField.text);

            messageInputField.text = string.Empty;
        }

        public async UniTask GenerateExceptionAsync()
        {
            if (!isJoin) { return; }
            
            await _streamingClient.GenerateException("client exception(streaminghub)!");
        }

        public void SampleMethod()
        {
            throw new System.NotImplementedException();
        }
        #endregion


        #region Server -> Client (Streaming)
        public void OnJoin(string name)
        {
            chatText.text += $"\n<color=grey>{name} entered the room.</color>";
        }


        public void OnLeave(string name)
        {
            chatText.text += $"\n<color=grey>{name} left the room.</color>";
        }

        public void OnSendMessage(MessageResponse message)
        {
            chatText.text += $"\n{message.UserName}: {message.Message}";
        }
        #endregion


        #region Client -> Server (Unary)
        
        private async UniTask SendReportAsync()
        {
            await _client.SendReportAsync(reportInputField.text);

            reportInputField.text = string.Empty;
        }
        
        private async UniTask UnaryGenerateExceptionAsync()
        {
            await _client.GenerateException("client exception(unary)£¡");
        }
        #endregion
    }
}
