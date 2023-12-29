using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HackingOps.UI
{
    public class Selector : MonoBehaviour
    {
        public event Action<int> OnChanged;

        [SerializeField] private TextMeshProUGUI _displayText;
        [SerializeField] private List<string> _values = new List<string>();

        private int _currentIndex;

        private void Awake()
        {
            if (_values.Count > 0)
            {
                DisplayElement();
            }
        }

        private void DisplayElement()
        {
            _displayText.text = _values[_currentIndex];
        }

        public void MoveForward()
        {
            if (_currentIndex >= _values.Count - 1) _currentIndex = 0;
            else _currentIndex++;

            DisplayElement();
            OnChanged?.Invoke(_currentIndex);
        }

        public void MoveBackward()
        {
            if (_currentIndex <= 0) _currentIndex = _values.Count - 1;
            else _currentIndex--;

            DisplayElement();
            OnChanged?.Invoke(_currentIndex);
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

        /// <summary>
        /// Move the selector to the index received
        /// </summary>
        /// <param name="index">Index of the selector's values</param>
        /// <returns>Returns true if it was possible to move to the desired index. Returns false otherwise</returns>
        public bool MoveTo(int index)
        {
            if (index < 0 && index >= _values.Count)
                return false;

            _currentIndex = index;
            DisplayElement();
            return true;
        }

        public void Add(string element)
        {
            _values.Add(element);
            DisplayElement();
        }

        public int GetSelection() => _currentIndex;
    }
}