using TMPro;
using UnityEngine;

namespace HackingOps.Screens.UI.ChatScreenElements
{
    public class MessageBubble : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}