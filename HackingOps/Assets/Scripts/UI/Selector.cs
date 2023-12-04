using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HackingOps.UI
{
    public class Selector : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _displayText;
        [SerializeField] private List<string> _values = new List<string>();

        private int _currentIndex;

        private void DisplayElement()
        {
            _displayText.text = _values[_currentIndex];
        }

        public void MoveForward()
        {
            if (_currentIndex >= _values.Count - 1) _currentIndex--;
            else _currentIndex++;

            DisplayElement();
        }

        public void MoveBackward()
        {
            if (_currentIndex <= 0) _currentIndex = _values.Count - 1;
            else _currentIndex--;

            DisplayElement();
        }

        /// <summary>
        /// Move to where the element received is found in the values list
        /// </summary>
        /// <param name="element">String to find in the values list</param>
        /// <returns>Return true if the element is found inside the values list. Return false otherwise</returns>
        public bool MoveTo(string element)
        {
            if (_values.Contains(element))
            {
                _currentIndex = _values.IndexOf(element);
                DisplayElement();
                return true;
            }

            return false;
        }

        public void Add(string element)
        {
            _values.Add(element);

            DisplayElement();
        }
    }
}