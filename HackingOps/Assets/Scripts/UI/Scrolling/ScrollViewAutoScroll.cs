using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HackingOps.UI.Scrolling
{
    public class ScrollViewAutoScroll : MonoBehaviour
    {
        [SerializeField] private float _scrollDuration = 0.2f;

        private ScrollRect _scrollRect;
        private Vector2 _nextScrollPosition = Vector2.up;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void ScrollToElement(GameObject[] elements, int elementIndex)
        {
            _nextScrollPosition = new Vector2(0, 1 - (elementIndex / ((float)elements.Length - 1)));

            DOVirtual.Vector2(_scrollRect.normalizedPosition, _nextScrollPosition, _scrollDuration, (x) =>
            {
                _scrollRect.normalizedPosition = x;
            });
        }
    }
}