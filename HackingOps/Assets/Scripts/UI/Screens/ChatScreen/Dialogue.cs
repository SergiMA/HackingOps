using System;
using UnityEngine;

namespace HackingOps.Screens.UI.ChatScreen
{
    public class Dialogue : MonoBehaviour
    {
        public event Action OnLastMessageShown;
        public event Action<MessageBubble[], int> OnNewMessageAppeared;

        public string Id => _id.Value;
        [SerializeField] private DialogueId _id;

        private MessageBubble[] _messageBubbles;
        private int _messageIndex;

        private void Awake()
        {
            _messageBubbles = GetComponentsInChildren<MessageBubble>();
        }

        private void Start()
        {
            HideAllMessages();
        }

        public void ShowNextMessage()
        {
            if (_messageIndex >= _messageBubbles.Length)
                return;

            _messageBubbles[_messageIndex].Show();
            OnNewMessageAppeared?.Invoke(_messageBubbles, _messageIndex);

            _messageIndex++;

            if (_messageIndex >= _messageBubbles.Length)
                OnLastMessageShown?.Invoke();

        }

        public void ShowAllMessages()
        {
            foreach (MessageBubble messageBubble in _messageBubbles)
                messageBubble.Show();

            _messageIndex = _messageBubbles.Length - 1;
        }

        public void HideAllMessages()
        {
            foreach (MessageBubble messageBubble in _messageBubbles)
                messageBubble.Hide();

            _messageIndex = 0;
        }
    }
}