using System.ComponentModel.Design;

namespace ConsoleSnake
{
    internal class Program
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private struct PlayArea
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

        private struct Position2D
        {
            public Position2D(Int32 x, Int32 y) { X = x; Y = y; }
            public Int32 X { get; set; }
            public Int32 Y { get; set; }
        }

        private static PlayArea _playArea = new PlayArea(1, 4, Console.WindowWidth - 2, Console.WindowHeight - 2);
        private static bool _running = true;
        private static bool _growing = false;
        private static Direction _direction = Direction.Up;
        private static List<Position2D> _snakeSegments = new List<Position2D>();
        private static Position2D _food = new Position2D(-1,-1);
        private static Random rng = new Random();
        static void Main(string[] args)
        {
            Console.ReadKey();

            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            DrawPlayArea();
            
            //Console.WindowWidth -= 1;
            Console.ReadKey(true);
            Reset();


            while (_running)
            {
                DateTime beginFrame = DateTime.Now;
                HandleInput();
                AllSnakeThings();
                TimeSpan frameTime = DateTime.Now.Subtract(beginFrame);

                if(50 - frameTime.Milliseconds > 0)
                    Thread.Sleep(50 - frameTime.Milliseconds);
            }
        }

        private static void DrawPlayArea()
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            for(Int32 top = 0; top < Console.WindowHeight; ++top)
            {
                Console.CursorTop = top;
                Console.CursorLeft = 0;
                if (top < _playArea.TopEdge || top > _playArea.BottomEdge)
                    for (Int32 left = 0; left < Console.WindowWidth; ++left)
                        Console.Write(' ');
                else
                {
                    for (Int32 left = 0; left < _playArea.LeftEdge; ++left)
                        Console.Write(' ');
                    //Console.CursorLeft = _playArea.RightEdge+1;
                    for (Int32 left = Console.CursorLeft = _playArea.RightEdge + 1; left < Console.WindowWidth; ++left)
                        Console.Write(' ');
                    /*Console.Write(' ');
                    Console.CursorLeft = Console.WindowWidth-1;
                    Console.Write(' ');*/
                }
            }
            /*for (Int32 top = _playArea.Top; top < _playArea.Height; ++top)
            {

            }
            for (Int32 left = _playArea.Left; left < _playArea.Width; ++left)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(' ');
            }*/
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private static void Reset()
        {
            _playArea = new PlayArea(1, 4, Console.WindowWidth - 2, Console.WindowHeight - 2);
            _running = true;
            _growing = false;
            _direction = Direction.Up;
            _snakeSegments.Clear();

            // Set up anew
            _snakeSegments.Add(new Position2D(Console.BufferWidth / 2, Console.BufferHeight / 2));
            SpawnFood();
            MoveCursor(_snakeSegments.Last());

        }

        private static void SpawnFood()
        {
            Position2D oldCursorPos = new Position2D(Console.CursorLeft, Console.CursorTop);
            do
            {
                _food = new Position2D(rng.Next(_playArea.LeftEdge+1, _playArea.RightEdge), rng.Next(_playArea.TopEdge, _playArea.BottomEdge));
            } while (_snakeSegments.Contains(_food));
            MoveCursor(_food);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write('@');
            Console.ForegroundColor = ConsoleColor.Green;
            MoveCursor(oldCursorPos);
        }

        private static void MoveCursor(Position2D pos) => MoveCursor(pos.X, pos.Y);
        private static void MoveCursor(Int32 left, Int32 top)
        {
            Console.SetCursorPosition(left, top);
            Console.CursorVisible = false;
        }

        private static void GrowSnake() => GrowSnake(Console.CursorLeft, Console.CursorTop);
        private static void GrowSnake(Int32 x, Int32 y)
        {
            _snakeSegments.Add(new Position2D(x, y));
        }

        private static void DeleteAt(Position2D pos) => DeleteAt(pos.X, pos.Y);
        private static void DeleteAt(Int32 x, Int32 y)
        {
            Int32 curX = Console.CursorLeft;
            Int32 curY = Console.CursorTop;
            MoveCursor(x, y);
            Console.Write(' ');
            MoveCursor(curX, curY);
        }

        private static bool Collision(Position2D pos1, Position2D pos2)
        {
            return (pos1.X == pos2.X && pos1.Y == pos2.Y);
        }

        private static void AllSnakeThings()
        {
            if (!_growing)
            {
                DeleteAt(_snakeSegments.First());
                _snakeSegments.Remove(_snakeSegments.First());
            }
            else
                _growing = false;
            ProcessMovement();
            if (_snakeSegments.Contains(new Position2D(Console.CursorLeft, Console.CursorTop)))
            {
                _running = false;
                return;
            }

            GrowSnake();
            Console.Write('#');
            --Console.CursorLeft;
            if(_food.X == Console.CursorLeft && _food.Y == Console.CursorTop)
            {
                _growing = true;
                SpawnFood();
            }
        }

        private static void ProcessMovement()
        {
            switch (_direction)
            {
                case Direction.Up:
                    if (Console.CursorTop - 1 < _playArea.TopEdge)
                        Console.CursorTop = _playArea.BottomEdge;
                    else
                        --Console.CursorTop;
                    break;
                case Direction.Down:
                    if (Console.CursorTop + 1 > _playArea.BottomEdge)
                        Console.CursorTop = _playArea.TopEdge;
                    else
                        ++Console.CursorTop;
                    break;
                case Direction.Left:
                    if (Console.CursorLeft - 1 < _playArea.LeftEdge)
                        Console.CursorLeft = _playArea.RightEdge;
                    else
                        --Console.CursorLeft;
                    break;
                case Direction.Right:
                    if (Console.CursorLeft + 1 > _playArea.RightEdge)
                        Console.CursorLeft = _playArea.LeftEdge;
                    else
                        ++Console.CursorLeft;
                    break;
            }
        }

        static void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                switch(Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if(_direction != Direction.Down)
                            _direction = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (_direction != Direction.Up)
                            _direction = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        if (_direction != Direction.Right)
                            _direction = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        if (_direction != Direction.Left)
                            _direction = Direction.Right;
                        break;
                    case ConsoleKey.Escape:
                        _running = false;
                        break;
                    case ConsoleKey.G:
                        _growing = !_growing;
                        break;
                }
            }
        }
    }
}