using HackingOps.Common.CommandSystem;
using HackingOps.Common.CommandSystem.Commands;
using HackingOps.Common.Services;
using HackingOps.Screens.UI.ChatScreenElements;
using UnityEngine;
using UnityEngine.Assertions;

namespace HackingOps.Screens.UI
{
    public class LaptopScreen : MonoBehaviour
    {
        private ChatScreen[] _chatScreens;

        private void Awake()
        {
            _chatScreens = GetComponentsInChildren<ChatScreen>(true);
        }

        private ChatScreen GetChatScreenByDialogue(DialogueId dialogueId) 
        {
            foreach (ChatScreen chatScreen in _chatScreens)
            {
                if (chatScreen.GetDialogue() == dialogueId)
                    return chatScreen;
            }

            return null;
        }

        public void OnCloseButtonPressed()
        {
            ServiceLocator.Instance.GetService<CommandQueue>().AddCommand(new StopUsingLaptopCommand());
        }

        public void OnLeaveHackingMode()
        {
            ServiceLocator.Instance.GetService<CommandQueue>().AddCommand(new StopUsingLaptopAfterHackingCommand());
        }

        public void LoadDialogue(DialogueId dialogueId)
        {
            ChatScreen chatScreen = GetChatScreenByDialogue(dialogueId);
            Assert.IsNotNull(chatScreen, $"<b>{name}</b> (LaptopScreen) can't get the <b>Chat Screen</b> for <b>{dialogueId}</b>. Make sure there's a Chat Screen with that ID");
            chatScreen.Show();
        }
    }
}