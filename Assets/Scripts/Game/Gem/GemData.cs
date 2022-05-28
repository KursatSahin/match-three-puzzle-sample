using System;
using Common;
using UnityEngine;

namespace Game.Gem
{
    public class GemData
    {
        public Action<Point, float> PositionChanged;
        public Action DestroyGem;
        
        private Point _position;
        private GemColor _color;
        private bool _isSelected;
        private bool _isSwapped;
        private bool _destroyed;
        private bool _isModified;

        public Point Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    var diff = Math.Max(_position.y - 8, 1);
                    var durationFactor = 1 + Mathf.Log(diff);
                    _position = value;
                    PositionChanged?.Invoke(value, durationFactor);
                }
            }
        }
        
        public bool Destroyed
        {
            get => _destroyed;
            set
            {
                if (_destroyed == value) return;

                _destroyed = value;
                
                if (value)
                {
                    DestroyGem?.Invoke();
                }
            }
        }

        public GemColor Color
        {
            get => _color;
            set => _color = value;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        public bool IsSwapped
        {
            get => _isSwapped;
            set => _isSwapped = value;
        }
        
        public bool IsModified
        {
            get => _isModified;
            set => _isModified = value;
        }
        
        public GemData(Point position, GemColor color)
        {
            _position = position;
            _color = color;
            _isSelected = false;
        }
    }

    public enum GemColor
    {
        Blue = 0,
        Green,
        Purple,
        Red,
        Yellow,
    }
}