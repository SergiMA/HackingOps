using UnityEngine;

namespace HackingOps.Characters.Common
{
    [CreateAssetMenu(menuName = "Hacking Ops/Characters/Create Character ID", fileName = "CharacterId")]
    public class CharacterId : ScriptableObject
    {
        [SerializeField] private string _value;
        public string Value => _value;
    }
}