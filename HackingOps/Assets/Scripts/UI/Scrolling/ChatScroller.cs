using HackingOps.Screens.UI.ChatScreen;
using UnityEngine;

namespace HackingOps.UI.Scrolling
{
    public class ChatScroller : MonoBehaviour
    {
        [SerializeField] private Dialogue[] _dialogues;
        [SerializeField] private ScrollViewAutoScroll _autoScroll;

        private void OnEnable()
        {
            foreach (Dialogue dialogue in _dialogues)
                dialogue.OnNewMessageAppeared += OnNewMessageAppeared;
        }

        private void OnDisable()
        {
            foreach (Dialogue dialogue in _dialogues)
                dialogue.OnNewMessageAppeared -= OnNewMessageAppeared;
        }

        private void OnNewMessageAppeared(MessageBubble[] messageBubbles, int messageIndex)
        {
            GameObject[] messageElements = new GameObject[messageBubbles.Length];

            for (int i = 0; i < messageBubbles.Length; i++)
            {
                messageElements[i] = messageBubbles[i].gameObject;
            }

            _autoScroll.ScrollToElement(messageElements, messageIndex);
        }
    }
}