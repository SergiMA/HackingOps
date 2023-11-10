using UnityEngine;

namespace HackingOps.Screens.UI.ChatScreenElements
{
    [CreateAssetMenu(fileName = "New Dialogue ID", menuName = "Hacking Ops/Dialogue ID")]
    public class DialogueId : ScriptableObject
    {
        [SerializeField] private string _value;
        public string Value => _value;
    }
}