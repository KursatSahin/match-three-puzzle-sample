using System;
using Common;

namespace Game.Gem
{
    public class GemData
    {
        private Point _position;
        private GemColor _color;
        private bool _isSelected;

        public Point Position
        {
            get => _position;
            set => _position = value;
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