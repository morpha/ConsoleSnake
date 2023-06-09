﻿using System.Xml.Linq;
using CC = ConsoleCompanion;

namespace ConsoleSnake
{
    /// <summary>
    /// Represents a playable snake entity.
    /// </summary>
    internal class Snake
    {
        private bool _alive = true;
        public bool Alive
        {
            get { return _alive; }
            set
            {
                if (!value)
                {
                    Segments.Clear();
                    HeadPosition = new Position2D(-1, -1);
                    _alive = false;
                    return;
                }
                _alive = true;
            }
        }

        //public Direction Direction { get; set; } = Direction.Down;
        public ConsoleColor Color { get; set; } = ConsoleColor.Green;
        public Position2D HeadPosition { get; set; } = new();
        public List<Position2D> Segments { get; set; } = new();

        public Vector2 Direction { get; set; } = Vector2.Down;

        public Snake(Int32 x, Int32 y) : this(new Position2D(x, y)) { }
        public Snake(Position2D startingPosition)
        {
            HeadPosition = startingPosition;
            Alive = true;
        }

        public void Movement(PlayArea playArea)
        {
            Position2D myPos = HeadPosition + Direction;

            myPos.X = Math.Max(playArea.LeftEdge, myPos.X % (playArea.RightEdge+1));
            myPos.Y = Math.Max(playArea.TopEdge, myPos.Y % (playArea.BottomEdge+1));

            
            //if (myPos.Y < playArea.TopEdge)
            //    myPos.Y = playArea.BottomEdge;

            //if (myPos.Y > playArea.BottomEdge)
            //    myPos.Y = playArea.TopEdge;

            //if (myPos.X < playArea.LeftEdge)
            //    myPos.X = playArea.RightEdge;

            //if (myPos.X > playArea.RightEdge)
            //    myPos.X = playArea.LeftEdge;

            HeadPosition = myPos;

        }

        /// <summary>
        /// Move the snake by the specified offset (vector).
        /// </summary>
        /// <param name="offset">The offset as Position2D to move the snake by.</param>
        public void MoveBy(Position2D offset)
        {
            HeadPosition += offset;
            Segments.ForEach((Position2D segment) =>
            {
                segment += offset;
            });
        }

        /// <summary>
        /// Move the snake to the specified location. The head will occupy the specified location, the body will retain its original shape and length relative to the head.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="constraint"></param>
        public bool MoveTo(Position2D newPosition, ISpatialConstraint2D? constraint = null)
        {
            if(constraint != null)
            {

            }
            HeadPosition = newPosition;
            return true;
        }

        /// <summary>
        /// Add the current head position as a segment to the snake.
        /// </summary>
        public void Grow() => Segments.Add(HeadPosition);

        public void Shift()
        {
            CC.Write(' ', Tail().X, Tail().Y, null, ConsoleColor.Black);
            Segments.RemoveAt(0);
            Grow();
        }

        /// <summary>
        /// Get the snake's head, short-hand for HeadPosition.
        /// </summary>
        /// <returns>Snake.HeadPosition</returns>
        public Position2D Head() => HeadPosition;

        /// <summary>
        /// Get the snake's tail segment.
        /// </summary>
        /// <returns>Position2D Snake.Segments[0]</returns>
        public Position2D Tail() => Segments[0];

        /// <summary>
        /// Kill this snake entity, emptying its segment-list and changing its alive state. Same as setting Alive to false.
        /// </summary>
        public void Kill() => Alive = false;

        public static Snake operator ++(Snake snake) { snake.Grow(); return snake; }
        
        public static implicit operator Int32(Snake snake) => snake.Segments.Count;
        public static implicit operator Position2D(Snake snake) => snake.HeadPosition;
    }
}
