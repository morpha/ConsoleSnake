using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSnake
{

    public interface ISpatialConstraint
    {
        bool IsInside<T>(T val);
        bool IsOutside<T>(T val);
    }
    /// <summary>
    /// Used to indicate the direction of movement (heading) of snake entities.
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    /// <summary>
    /// Intended to be used as return type for collision detection methods.
    /// </summary>
    public enum CollisionType
    {
        None,
        Food,
        Body,
        Head,
        Wall
    }
    public struct PlayArea : ISpatialConstraint
    {
        public PlayArea(Int32 left, Int32 top, Int32 right, Int32 bottom)
        {
            LeftEdge = left;
            TopEdge = top;
            RightEdge = right;
            BottomEdge = bottom;
        }
        public Int32 LeftEdge { get; set; }
        public Int32 TopEdge { get; set; }
        public Int32 RightEdge { get; set; }
        public Int32 BottomEdge { get; set; }

        public bool IsInside<T>(T pos)
        {
            if(typeof(T) == typeof(Position2D))
            {
                Console.WriteLine("it's a pos2d");
                return true;
            }
            Console.WriteLine("it's NOT a pos2d");
            return false;
        }

        public bool IsOutside<T>(T pos) => !IsInside(pos);
    }

    public struct Position2D
    {
        public Position2D(Int32 x = 0, Int32 y = 0) { X = x; Y = y; }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }

        public static Position2D operator +(Position2D left, Int32 right) => new Position2D(left.X + right, left.Y + right);
        public static Position2D operator -(Position2D left, Int32 right) => new Position2D(left.X - right, left.Y - right);
        public static Position2D operator +(Position2D left, Position2D right) => new Position2D(left.X + right.X, left.Y + right.Y);
        public static Position2D operator -(Position2D left, Position2D right) => new Position2D(left.X - right.X, left.Y - right.Y);
        public static Position2D operator ++(Position2D pos) => pos += 1;
        public static Position2D operator --(Position2D pos) => pos -= 1;
    }
}
