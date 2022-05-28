using System;

namespace Common
{
    public struct Point : IEquatable<Point>
    {
        #region Static Fields

        public static readonly Point Right = new Point(1, 0);
        public static readonly Point Up = new Point(0, 1);
        public static readonly Point Left = new Point(-1, 0);
        public static readonly Point Down = new Point(0, -1);
        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point Null = new Point(int.MinValue, int.MinValue);
        
        #endregion

        #region Public Fields
        
        public int x;
        public int y;

        #endregion

        #region Public Functions

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object other)
        {
            if (other is Point otherCoordinate)      
                return Equals((Point)other);
            return false;
        }
    
        public bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }
    
        public override int GetHashCode()
        {
            return x ^ y;
        }

        public override string ToString()
        {
            return $"Coordinate({x}, {y})";
        }

        #endregion

        #region Operator Overloads
        public static bool operator ==(Point coord1, Point coord2)
        {
            float xDiff = coord1.x - coord2.x;
            float yDiff = coord1.y - coord2.y;
            return xDiff == 0 && yDiff == 0;
        }

        public static bool operator !=(Point coord1, Point coord2)
        {
            return !(coord1 == coord2);
        }

        public static Point operator +(Point coord1, Point coord2)
        {
            return new Point(coord1.x + coord2.x, coord1.y + coord2.y);
        }

        public static Point operator -(Point coord1, Point coord2)
        {
            return new Point(coord1.x - coord2.x, coord1.y - coord2.y);
        }

        public static Point operator *(Point coord1, int factor)
        {
            return new Point(coord1.x * factor, coord1.y * factor);
        }
        
        #endregion
    }
}
