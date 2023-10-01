using UnityEngine;

namespace HackingOps.CutsceneSystem
{
    public class CutsceneIdentification : MonoBehaviour
    {
        [SerializeField] private CutsceneId _id;
        public string Id => _id.Value;
    }
}