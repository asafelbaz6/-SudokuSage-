namespace Point
{
    public class Point
    {
        public int X {get; set;}
        public int Y {get; set;}

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public Point()
        {
            X = 0;
            Y = 0;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Point other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Using XOR for better distribution of hash codes
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}