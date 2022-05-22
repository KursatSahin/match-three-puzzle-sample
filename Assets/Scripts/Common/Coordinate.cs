using System;

namespace Common
{
    public class Coordinate : IEquatable<Coordinate>
    {
        public int x;
        public int y;

        public Coordinate()
        {
            this.x = 0;
            this.y = 0;
        }

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object other)
        {
            if (other is Coordinate otherCoordinate)      
                return Equals((Coordinate)other);
            return false;
        }
    
        public bool Equals(Coordinate other)
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
        
        /// Operator overloads
        
        public static bool operator ==(Coordinate coord1, Coordinate coord2)
        {
            float xDiff = coord1.x - coord2.x;
            float yDiff = coord1.y - coord2.y;
            return xDiff == 0 && yDiff == 0;
        }

        public static bool operator !=(Coordinate coord1, Coordinate coord2)
        {
            return !(coord1 == coord2);
        }

        public static Coordinate operator +(Coordinate coord1, Coordinate coord2)
        {
            return new Coordinate(coord1.x + coord2.x, coord1.y + coord2.y);
        }

        public static Coordinate operator -(Coordinate coord1, Coordinate coord2)
        {
            return new Coordinate(coord1.x - coord2.x, coord1.y - coord2.y);
        }

        public static Coordinate operator *(Coordinate coord1, int factor)
        {
            return new Coordinate(coord1.x * factor, coord1.y * factor);
        }
    }
}
