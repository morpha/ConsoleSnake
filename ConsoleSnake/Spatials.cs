using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSnake
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    public struct PlayArea
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
    }

    public struct Position2D
    {
        public Position2D() : this(-1, -1) { }
        public Position2D(Int32 x, Int32 y) { X = x; Y = y; }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
    }
}
