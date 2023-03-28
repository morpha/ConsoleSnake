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
            public Position2D() : this(-1, -1) { }
            public Position2D(Int32 x, Int32 y) { X = x; Y = y; }
            public Int32 X { get; set; }
            public Int32 Y { get; set; }
        }

        private struct Snake
        {
            public Direction Direction { get; set; } = Direction.Down;
            public Position2D HeadPosition { get; set; } = new Position2D();
            public List<Position2D> Segments { get; set; } = new List<Position2D>();
            public bool Growing { get; set; } = false;

            public Snake()
            {
            }
        }

        private static PlayArea _playArea = new PlayArea(1, 4, Console.WindowWidth - 2, Console.WindowHeight - 2);
        private static bool _running = true;
        private static Snake _snakeP1 = new Snake();
        private static Snake _snakeP2 = new Snake();
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
                    for (Int32 left = Console.CursorLeft = _playArea.RightEdge + 1; left < Console.WindowWidth; ++left)
                        Console.Write(' ');
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
        }
        private static void Reset()
        {
            _playArea = new PlayArea(1, 4, Console.WindowWidth - 2, Console.WindowHeight - 2);
            _running = true;
            _snakeP1.Growing = false;
            _snakeP2.Growing = false;
            _snakeP1.Direction = Direction.Right;
            _snakeP2.Direction = Direction.Left;
            _snakeP1.Segments.Clear();
            _snakeP2.Segments.Clear();

            // Set up anew
            _snakeP1.Segments.Add(_snakeP1.HeadPosition = new Position2D(Console.BufferWidth / 2, Console.BufferHeight / 2));
            _snakeP2.Segments.Add(_snakeP2.HeadPosition = new Position2D(Console.BufferWidth / 2 - 1, Console.BufferHeight / 2));
            SpawnFood();
        }

        private static void SpawnFood()
        {
            //Position2D oldCursorPos = new Position2D(Console.CursorLeft, Console.CursorTop);
            do
            {
                _food = new Position2D(rng.Next(_playArea.LeftEdge+1, _playArea.RightEdge), rng.Next(_playArea.TopEdge, _playArea.BottomEdge));
            } while (_snakeP1.Segments.Contains(_food) || _snakeP2.Segments.Contains(_food));
            MoveCursor(_food);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write('@');
            //MoveCursor(oldCursorPos);
        }

        private static void WriteColAt(char text, ConsoleColor color, Position2D pos) => WriteColAt(text.ToString(), color, pos.X, pos.Y);
        private static void WriteColAt(string text, ConsoleColor color, Position2D pos) => WriteColAt(text, color, pos.X, pos.Y);
        private static void WriteColAt(char text, ConsoleColor color, Int32 x, Int32 y) => WriteColAt(text.ToString(), color, x, y);
        private static void WriteColAt(string text, ConsoleColor color, Int32 x, Int32 y)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(text);
        }

        private static void MoveCursor(Position2D pos) => MoveCursor(pos.X, pos.Y);
        private static void MoveCursor(Int32 left, Int32 top)
        {
            Console.SetCursorPosition(left, top);
            Console.CursorVisible = false;
        }
        private static void GrowSnakes()
        {
            if (_snakeP1.Segments.Count > 0)
            {
                _snakeP1.Segments.Add(_snakeP1.HeadPosition);
                WriteColAt('#', ConsoleColor.Green, _snakeP1.HeadPosition);
            }
            if (_snakeP2.Segments.Count > 0)
            {
                _snakeP2.Segments.Add(_snakeP2.HeadPosition);
                WriteColAt('#', ConsoleColor.Cyan, _snakeP2.HeadPosition);
            }
        }

        private static void DeleteAt(Position2D pos) => DeleteAt(pos.X, pos.Y);
        private static void DeleteAt(Int32 x, Int32 y)
        {
            MoveCursor(x, y);
            Console.Write(' ');
        }

        private static bool Collision(Position2D pos1, Position2D pos2)
        {
            return (pos1.X == pos2.X && pos1.Y == pos2.Y);
        }

        private static void AllSnakeThings()
        {

            ProcessMovement();

            // P1 pepsi?
            if (_snakeP1.Segments.Contains(_snakeP1.HeadPosition) || _snakeP2.Segments.Contains(_snakeP1.HeadPosition))
            {
                foreach(Position2D segment in _snakeP1.Segments)
                    DeleteAt(segment);
                _snakeP1.Segments.Clear();
                return;
            }
            // P2 pepsi?
            if (_snakeP1.Segments.Contains(_snakeP2.HeadPosition) || _snakeP2.Segments.Contains(_snakeP2.HeadPosition))
            {
                foreach (Position2D segment in _snakeP2.Segments)
                    DeleteAt(segment);
                _snakeP2.Segments.Clear();
                return;
            }
            // everyone pepsi? game over?!
            if(_snakeP1.Segments.Count == 0 && _snakeP2.Segments.Count == 0)
                _running = false;

            GrowSnakes();

            if(Collision(_food, _snakeP1.HeadPosition))
            {
                _snakeP1.Growing = true;
                SpawnFood();
            } else if (Collision(_food, _snakeP2.HeadPosition))
            {
                _snakeP2.Growing = true;
                SpawnFood();
            }

            // growth P1
            if (!_snakeP1.Growing && _snakeP1.Segments.Count > 0)
            {
                DeleteAt(_snakeP1.Segments.First());
                _snakeP1.Segments.Remove(_snakeP1.Segments.First());
            }
            else
                _snakeP1.Growing = false;
            // growth P2
            if (!_snakeP2.Growing && _snakeP2.Segments.Count > 0)
            {
                DeleteAt(_snakeP2.Segments.First());
                _snakeP2.Segments.Remove(_snakeP2.Segments.First());
            }
            else
                _snakeP2.Growing = false;
        }

        private static void ProcessMovement()
        {
            Position2D posP1 = _snakeP1.HeadPosition;
            Position2D posP2 = _snakeP2.HeadPosition;

            switch (_snakeP1.Direction)
            {
                case Direction.Up:
                    if (posP1.Y - 1 < _playArea.TopEdge)
                        posP1.Y = _playArea.BottomEdge;
                    else
                        --posP1.Y;
                    break;
                case Direction.Down:
                    if (posP1.Y + 1 > _playArea.BottomEdge)
                        posP1.Y = _playArea.TopEdge;
                    else
                        ++posP1.Y;
                    break;
                case Direction.Left:
                    if (posP1.X - 1 < _playArea.LeftEdge)
                        posP1.X = _playArea.RightEdge;
                    else
                        --posP1.X;
                    break;
                case Direction.Right:
                    if (posP1.X + 1 > _playArea.RightEdge)
                        posP1.X = _playArea.LeftEdge;
                    else
                        ++posP1.X;
                    break;
            }
            switch (_snakeP2.Direction)
            {
                case Direction.Up:
                    if (posP2.Y - 1 < _playArea.TopEdge)
                        posP2.Y = _playArea.BottomEdge;
                    else
                        --posP2.Y;
                    break;
                case Direction.Down:
                    if (posP2.Y + 1 > _playArea.BottomEdge)
                        posP2.Y = _playArea.TopEdge;
                    else
                        ++posP2.Y;
                    break;
                case Direction.Left:
                    if (posP2.X - 1 < _playArea.LeftEdge)
                        posP2.X = _playArea.RightEdge;
                    else
                        --posP2.X;
                    break;
                case Direction.Right:
                    if (posP2.X + 1 > _playArea.RightEdge)
                        posP2.X = _playArea.LeftEdge;
                    else
                        ++posP2.X;
                    break;
            }

            _snakeP1.HeadPosition = posP1;
            _snakeP2.HeadPosition = posP2;
        }

        static void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                switch(Console.ReadKey(true).Key)
                {
                    // Movement P1
                    case ConsoleKey.UpArrow:
                        if(_snakeP1.Direction != Direction.Down)
                            _snakeP1.Direction = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        if (_snakeP1.Direction != Direction.Up)
                            _snakeP1.Direction = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (_snakeP1.Direction != Direction.Right)
                            _snakeP1.Direction = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (_snakeP1.Direction != Direction.Left)
                            _snakeP1.Direction = Direction.Right;
                        break;

                    // Movement P2
                    case ConsoleKey.W:
                        if (_snakeP2.Direction != Direction.Down)
                            _snakeP2.Direction = Direction.Up;
                        break;
                    case ConsoleKey.S:
                        if (_snakeP2.Direction != Direction.Up)
                            _snakeP2.Direction = Direction.Down;
                        break;
                    case ConsoleKey.A:
                        if (_snakeP2.Direction != Direction.Right)
                            _snakeP2.Direction = Direction.Left;
                        break;
                    case ConsoleKey.D:
                        if (_snakeP2.Direction != Direction.Left)
                            _snakeP2.Direction = Direction.Right;
                        break;

                    case ConsoleKey.Escape:
                        _running = false;
                        break;
                    case ConsoleKey.G:
                        _snakeP1.Growing = !_snakeP1.Growing;
                        break;
                    case ConsoleKey.H:
                        _snakeP2.Growing = !_snakeP2.Growing;
                        break;
                }
            }
        }
    }
}