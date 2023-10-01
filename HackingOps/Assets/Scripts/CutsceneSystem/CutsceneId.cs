using UnityEngine;

namespace HackingOps.CutsceneSystem
{
    [CreateAssetMenu(menuName = "Hacking Ops/Cutscenes/Create Cutscene ID", fileName = "Cutscene ID")]
    public class CutsceneId : ScriptableObject
    {
        [SerializeField] private string _value;
        public string Value => _value;
    }
}