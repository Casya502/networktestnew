using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChatUI
{
    public struct ChatMessage : IBroadcast
    {
        public string Sender;
        public string Message;
    }

    public class ChatPanel : MonoBehaviour
    {
        public static ChatPanel Instance;
        [SerializeField] private RectTransform content;
        private Button[] _buttons;
        [SerializeField] private TMP_InputField messageInput;
        public int totalLineCount = 0;

        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private int lineHeight = 40;


        private void Awake()
        {
            Instance = this;
            _buttons = GetComponentsInChildren<Button>(true);
        }

        private void OnEnable()
        {
            _buttons[0].onClick.AddListener(SendChatMessage);
            messageInput.onSubmit.AddListener(SendChatMessage);
            InstanceFinder.ClientManager.RegisterBroadcast<ChatMessage>(OnClientChatMessageReceived);
            InstanceFinder.ServerManager.RegisterBroadcast<ChatMessage>(OnServerChatMessageReceived);
        }

        private void OnDisable()
        {
            Clear();
            _buttons[0].onClick.RemoveListener(SendChatMessage);
            messageInput.onSubmit.RemoveListener(SendChatMessage);
            InstanceFinder.ClientManager.UnregisterBroadcast<ChatMessage>(OnClientChatMessageReceived);
            InstanceFinder.ServerManager.UnregisterBroadcast<ChatMessage>(OnServerChatMessageReceived);
        }

        private void OnClientChatMessageReceived(ChatMessage chatMessage, Channel channel)
        {
            SpawnMsg(chatMessage);
        }

        private void OnServerChatMessageReceived(
            NetworkConnection networkConnection,
            ChatMessage chatMessage,
            Channel channel)
        {
            InstanceFinder.ServerManager.Broadcast(chatMessage);
        }

        private void SpawnMsg(ChatMessage chatMessage)
        {
            if (string.IsNullOrEmpty(chatMessage.Message))
            {
                return;
            }

            GameObject newMessage = Instantiate(messagePrefab, content, false);
            Vector2 pos = newMessage.transform.localPosition;
            pos.y = -totalLineCount * lineHeight;
            newMessage.transform.localPosition = pos;
            totalLineCount += newMessage.GetComponent<Message>().Init(chatMessage.Sender, chatMessage.Message) + 1;
            Vector2 size = content.sizeDelta;
            size.y = lineHeight * (totalLineCount + 2);
            content.sizeDelta = size;
            if (size.y >= 700)
                content.localPosition = new Vector3(0, size.y - 700);
        }

        private void SendChatMessage()
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Sender = "233:",
                Message = messageInput.text
            };
            if (InstanceFinder.IsServerStarted)
            {
                InstanceFinder.ServerManager.Broadcast(chatMessage);
                messageInput.text = null;
            }
            else if (InstanceFinder.IsClientStarted)
            {
                InstanceFinder.ClientManager.Broadcast(chatMessage);
                messageInput.text = null;
            }
        }

        private void SendChatMessage(string message)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Sender = "233:",
                Message = message
            };
            if (InstanceFinder.IsServerStarted)
            {
                InstanceFinder.ServerManager.Broadcast(chatMessage);
                messageInput.text = null;
            }
            else if (InstanceFinder.IsClientStarted)
            {
                InstanceFinder.ClientManager.Broadcast(chatMessage);
                messageInput.text = null;
            }
        }

        public void Clear()
        {
            (content as Transform).DestroyAllChildren();
            Vector3 size = content.sizeDelta;
            size.y = 700;
            content.sizeDelta = size;
            content.localPosition = new Vector3(0, 0);
            totalLineCount = 0;
        }
    }
}