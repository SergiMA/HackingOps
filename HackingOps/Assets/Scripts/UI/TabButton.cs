using UnityEngine;

namespace HackingOps.UI
{
    public class TabButton : MonoBehaviour
    {
        [field:SerializeField] public CanvasGroup TabContent { get; private set; }
    }
}