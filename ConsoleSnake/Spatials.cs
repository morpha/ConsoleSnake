using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSnake
{

    public interface ISpatialConstraint2D
    {
        bool IsInside(Position2D pos);
        bool IsOutside(Position2D pos);
    }
    /// <summary>
    /// Used to indicate the direction of movement (heading) of entities.
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

    internal struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2() : this(0.0f, 0.0f) { }
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 Zero => new Vector2(0.0f, 0.0f);

        public static Vector2 UnitX => new Vector2(1.0f, 0.0f);
        public static Vector2 UnitY => new Vector2(0.0f, 1.0f);

        public static Vector2 Up => new Vector2(0.0f, -1.0f);
        public static Vector2 Right => new Vector2(1.0f, 0.0f);
        public static Vector2 Down => new Vector2(0.0f, 1.0f);
        public static Vector2 Left => new Vector2(-1.0f, 0.0f);

        public static Vector2 North => Up;
        public static Vector2 East => Right;
        public static Vector2 South => Down;
        public static Vector2 West => Left;

        public static bool operator ==(Vector2 left, Vector2 right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(Vector2 left, Vector2 right) => left.X != right.X || left.Y != right.Y;

        public override bool Equals(object? obj)
        {
            if((obj == null) || obj.GetType() != this.GetType())
                return false;

            return (Vector2)obj == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y);
        }

        public static implicit operator Position2D(Vector2 v) => new((Int32)v.X, (Int32)v.Y);
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

        public override string ToString() => $"Position2D[X:{X}, Y:{Y}]";
    }

    public struct PlayArea : ISpatialConstraint2D
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

        public bool IsInside(Position2D pos) => (LeftEdge <= pos.X && pos.X <= RightEdge && TopEdge <= pos.Y && pos.Y <= BottomEdge);

        public bool IsOutside(Position2D pos) => !IsInside(pos);
    }
}
