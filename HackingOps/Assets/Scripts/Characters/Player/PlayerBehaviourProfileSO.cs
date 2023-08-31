using UnityEngine;

namespace HackingOps.Characters.Player
{
    [CreateAssetMenu(fileName = "new Player Behaviour Profile", menuName = "Hacking Ops/Characters/Player Behaviour Profile")]
    public class PlayerBehaviourProfileSO : ScriptableObject
    {
        [Header("Movement settings")]
        public PlayerController.MovementMode MovementMode;

        [Header("Orientation settings")]
        public PlayerController.OrientationMode OrientationMode;
        public bool OrientateAlways;
    }
}