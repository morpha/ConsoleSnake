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
        private static bool _growingP1 = false;
        private static bool _growingP2 = false;
        private static Direction _directionP1 = Direction.Right;
        private static Direction _directionP2 = Direction.Left;
        private static Position2D _headP1 = new Position2D(-1, -1);
        private static Position2D _headP2 = new Position2D(-1, -1);
        private static List<Position2D> _snakeSegmentsP1 = new List<Position2D>();
        private static List<Position2D> _snakeSegmentsP2 = new List<Position2D>();
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
            _growingP1 = false;
            _growingP2 = false;
            _directionP1 = Direction.Right;
            _directionP2 = Direction.Left;
            _snakeSegmentsP1.Clear();
            _snakeSegmentsP2.Clear();

            // Set up anew
            _snakeSegmentsP1.Add(_headP1 = new Position2D(Console.BufferWidth / 2, Console.BufferHeight / 2));
            _snakeSegmentsP2.Add(_headP2 = new Position2D(Console.BufferWidth / 2 - 1, Console.BufferHeight / 2));
            SpawnFood();
            MoveCursor(_snakeSegmentsP1.Last());

        }

        private static void SpawnFood()
        {
            Position2D oldCursorPos = new Position2D(Console.CursorLeft, Console.CursorTop);
            do
            {
                _food = new Position2D(rng.Next(_playArea.LeftEdge+1, _playArea.RightEdge), rng.Next(_playArea.TopEdge, _playArea.BottomEdge));
            } while (_snakeSegmentsP1.Contains(_food) || _snakeSegmentsP2.Contains(_food));
            MoveCursor(_food);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write('@');
            Console.ForegroundColor = ConsoleColor.Green;
            MoveCursor(oldCursorPos);
        }

        private static void JoinPlayer2()
        {

        }

        private static void MoveCursor(Position2D pos) => MoveCursor(pos.X, pos.Y);
        private static void MoveCursor(Int32 left, Int32 top)
        {
            Console.SetCursorPosition(left, top);
            Console.CursorVisible = false;
        }
        private static void GrowSnakes()
        {
            if (_snakeSegmentsP1.Count > 0)
            {
                _snakeSegmentsP1.Add(_headP1);
                Console.SetCursorPosition(_headP1.X, _headP1.Y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('#');
            }
            if (_snakeSegmentsP2.Count > 0)
            {
                _snakeSegmentsP2.Add(_headP2);
                Console.SetCursorPosition(_headP2.X, _headP2.Y);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write('#');
            }
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

            ProcessMovement();

            if (_snakeSegmentsP1.Contains(_headP1) || _snakeSegmentsP2.Contains(_headP1))
            {
                foreach(Position2D segment in _snakeSegmentsP1)
                    DeleteAt(segment);
                _snakeSegmentsP1.Clear();
                return;
            }
            if (_snakeSegmentsP1.Contains(_headP2) || _snakeSegmentsP2.Contains(_headP2))
            {
                foreach (Position2D segment in _snakeSegmentsP2)
                    DeleteAt(segment);
                _snakeSegmentsP2.Clear();
                return;
            }
            if(_snakeSegmentsP1.Count == 0 && _snakeSegmentsP2.Count == 0)
                _running = false;

            GrowSnakes();

            if(_food.X == _headP1.X && _food.Y == _headP1.Y)
            {
                _growingP1 = true;
                SpawnFood();
            } else if (_food.X == _headP2.X && _food.Y == _headP2.Y)
            {
                _growingP2 = true;
                SpawnFood();
            }

            // growth P1
            if (!_growingP1 && _snakeSegmentsP1.Count > 0)
            {
                DeleteAt(_snakeSegmentsP1.First());
                _snakeSegmentsP1.Remove(_snakeSegmentsP1.First());
            }
            else
                _growingP1 = false;
            // growth P2
            if (!_growingP2 && _snakeSegmentsP2.Count > 0)
            {
                DeleteAt(_snakeSegmentsP2.First());
                _snakeSegmentsP2.Remove(_snakeSegmentsP2.First());
            }
            else
                _growingP2 = false;/**/
        }

        private static void ProcessMovement()
        {
            /*switch (_directionP1)
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
            }/**/
            switch (_directionP1)
            {
                case Direction.Up:
                    if (_headP1.Y - 1 < _playArea.TopEdge)
                        _headP1.Y = _playArea.BottomEdge;
                    else
                        --_headP1.Y;
                    break;
                case Direction.Down:
                    if (_headP1.Y + 1 > _playArea.BottomEdge)
                        _headP1.Y = _playArea.TopEdge;
                    else
                        ++_headP1.Y;
                    break;
                case Direction.Left:
                    if (_headP1.X - 1 < _playArea.LeftEdge)
                        _headP1.X = _playArea.RightEdge;
                    else
                        --_headP1.X;
                    break;
                case Direction.Right:
                    if (_headP1.X + 1 > _playArea.RightEdge)
                        _headP1.X = _playArea.LeftEdge;
                    else
                        ++_headP1.X;
                    break;
            }
            switch (_directionP2)
            {
                case Direction.Up:
                    if (_headP2.Y - 1 < _playArea.TopEdge)
                        _headP2.Y = _playArea.BottomEdge;
                    else
                        --_headP2.Y;
                    break;
                case Direction.Down:
                    if (_headP2.Y + 1 > _playArea.BottomEdge)
                        _headP2.Y = _playArea.TopEdge;
                    else
                        ++_headP2.Y;
                    break;
                case Direction.Left:
                    if (_headP2.X - 1 < _playArea.LeftEdge)
                        _headP2.X = _playArea.RightEdge;
                    else
                        --_headP2.X;
                    break;
                case Direction.Right:
                    if (_headP2.X + 1 > _playArea.RightEdge)
                        _headP2.X = _playArea.LeftEdge;
                    else
                        ++_headP2.X;
                    break;
            }
        }

        static void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                switch(Console.ReadKey(true).Key)
                {
                    // Movement P1
                    case ConsoleKey.UpArrow:
                        if(_directionP1 != Direction.Down)
                            _directionP1 = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        if (_directionP1 != Direction.Up)
                            _directionP1 = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (_directionP1 != Direction.Right)
                            _directionP1 = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (_directionP1 != Direction.Left)
                            _directionP1 = Direction.Right;
                        break;

                    // Movement P2
                    case ConsoleKey.W:
                        if (_directionP2 != Direction.Down)
                            _directionP2 = Direction.Up;
                        break;
                    case ConsoleKey.S:
                        if (_directionP2 != Direction.Up)
                            _directionP2 = Direction.Down;
                        break;
                    case ConsoleKey.A:
                        if (_directionP2 != Direction.Right)
                            _directionP2 = Direction.Left;
                        break;
                    case ConsoleKey.D:
                        if (_directionP2 != Direction.Left)
                            _directionP2 = Direction.Right;
                        break;

                    case ConsoleKey.D2:
                        JoinPlayer2();
                        break;

                    case ConsoleKey.Escape:
                        _running = false;
                        break;
                    case ConsoleKey.G:
                        _growingP1 = !_growingP1;
                        break;
                }
            }
        }
    }
}