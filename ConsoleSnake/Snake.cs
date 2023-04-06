using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleSnake
{
    class Snake
    {
        public bool Alive = true;
        public Direction Direction { get; set; } = Direction.Down;
        public ConsoleColor Color { get; set; } = ConsoleColor.Green;
        public Position2D HeadPosition { get; set; } = new Position2D();
        public List<Position2D> Segments { get; set; } = new List<Position2D>();

        public Snake()
        {
        }

        public void Grow()
        {
            Segments.Add(HeadPosition);
        }
        public void Kill()
        {
            Segments.Clear();
            HeadPosition = new Position2D(-1, -1);
            Alive = false;
        }

        public static Snake operator ++(Snake snake) { snake.Grow(); return snake; }
        public static bool operator >(Snake left, Snake right) => left.Segments.Count > right.Segments.Count;
        public static bool operator <(Snake left, Snake right) => left.Segments.Count < right.Segments.Count;
        public static bool operator >=(Snake left, Snake right) => left.Segments.Count >= right.Segments.Count;
        public static bool operator <=(Snake left, Snake right) => left.Segments.Count <= right.Segments.Count;
    }
}
