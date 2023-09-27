using UnityEngine;

namespace HackingOps.Characters.Common
{
    public class CharacterIdentification : MonoBehaviour
    {
        [SerializeField] private CharacterId _id;
        public string Id => _id.Value;
    }
}