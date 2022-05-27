using System;
using Common;

namespace Game.Gem
{
    public class GemData
    {
        public Action<Point> PositionChanged;
        public Action DestroyGem;
        
        private Point _position;
        private GemColor _color;
        private bool _isSelected;
        private bool _isSwapped;
        private bool _destroyed;


        public Point Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    PositionChanged?.Invoke(value);
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