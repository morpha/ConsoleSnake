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

        private class Snake
        {
            public bool Alive = true;
            public Direction Direction { get; set; } = Direction.Down;
            public ConsoleColor Color { get; set; } = ConsoleColor.Green;
            public Position2D HeadPosition { get; set; } = new Position2D();
            public List<Position2D> Segments { get; set; } = new List<Position2D>();

            public Snake()
            {
            }
        }

        const Int32 MINWIDTH = 82;
        const Int32 MINHEIGHT = 35;

        private static PlayArea _playArea = new PlayArea(1, 4, Console.WindowWidth - 2, Console.WindowHeight - 2);

        private static bool _gameRunning = true;
        private static bool _running = true;
        
        private static Snake[] _snakes = { 
            new Snake(),
            new Snake()
        };
        private static Position2D _food = new Position2D(-1,-1);
        private static Random rng = new Random();
        private static Int32 _desiredFrameTime = 75;

        private static DateTime _frameTiming = DateTime.Now;

        static void Setup()
        {
            Console.SetWindowSize(MINWIDTH, MINHEIGHT);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            Console.CursorVisible = false;
        }
        static void Main(string[] args)
        {
            Console.ReadKey();
            Setup();
            AsciiArt.IntroAnimation();
            MainLoop();
        }

        private static void ShowMenu()
        {
            Menu menu = new Menu();
            menu.AddItem("Play [1P]", "Play a single-player game.", () => { return false; });
            menu.AddItem("Play [2P]", "Play a two-player game.", () => { return false; });
            menu.AddItem("Quit", "Quit the game.", () => { return _gameRunning = false; });
            menu.Show();
        }

        private static void MainLoop()
        {
            //foreach (ConsoleColor col in Enum.GetValues(typeof(ConsoleColor)))
            //{
            //    Console.ForegroundColor = col;
            //    Console.WriteLine(col);
            //}
            while (_gameRunning)
            {
                ShowMenu();
                Reset();
                DrawPlayArea();
                AsciiArt.Countdown(col: ConsoleColor.Magenta);
                while (_running)
                {
                    TimeFrame();
                    HandleInput();
                    AllSnakeThings();
                }
            }
        }

        private static void TimeFrame()
        {
            TimeSpan frameTime = DateTime.Now.Subtract(_frameTiming);
            if(frameTime.Milliseconds < _desiredFrameTime)
            {
                Thread.Sleep(_desiredFrameTime - frameTime.Milliseconds);
            }
            _frameTiming = DateTime.Now;
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
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            _playArea = new PlayArea(1, 4, Console.WindowWidth - 2, Console.WindowHeight - 2);
            _running = true;

            _snakes[0].Direction = Direction.Right;
            _snakes[1].Direction = Direction.Left;
            _snakes[0].Color = ConsoleColor.Green;
            _snakes[1].Color = ConsoleColor.Cyan;
            _snakes[0].Segments.Clear();
            _snakes[1].Segments.Clear();
            _snakes[0].Alive = true;
            _snakes[1].Alive = true;

            // Set up anew
            _snakes[0].Segments.Add(_snakes[0].HeadPosition = new Position2D(Console.BufferWidth / 2, Console.BufferHeight / 2));
            _snakes[1].Segments.Add(_snakes[1].HeadPosition = new Position2D(Console.BufferWidth / 2 - 1, Console.BufferHeight / 2));
            SpawnFood();
        }

        private static bool SnakeCollision(Position2D position)
        {
            return _snakes[0].Segments.Contains(position) || _snakes[1].Segments.Contains(position);
        }

        private static void SpawnFood()
        {
            while (SnakeCollision(_food = new Position2D(rng.Next(_playArea.LeftEdge + 1, _playArea.RightEdge), rng.Next(_playArea.TopEdge, _playArea.BottomEdge))))
            {

            }

            WriteColAt('@', ConsoleColor.Red, _food);
        }

        private static void WriteColAt(object text, ConsoleColor color, Position2D pos) => WriteColAt(text.ToString(), color, pos.X, pos.Y);
        private static void WriteColAt(string text, ConsoleColor color, Position2D pos) => WriteColAt(text, color, pos.X, pos.Y);
        private static void WriteColAt(object text, ConsoleColor color, Int32 x, Int32 y) => WriteColAt(text.ToString(), color, x, y);
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
        private static void GrowSnake(Snake snake)
        {
            snake.Segments.Add(snake.HeadPosition);
            WriteColAt('#', snake.Color, snake.HeadPosition);
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

        private static void KillSnake(Snake snake)
        {
            foreach (Position2D segment in snake.Segments)
                DeleteAt(segment);
            snake.Segments.Clear();
            snake.HeadPosition = new Position2D(-1, -1);
            snake.Alive = false;
        }

        private static void AllSnakeThings()
        {

            ProcessMovement();

            foreach (Snake snake in _snakes)
            {
                if (snake.Alive)
                {
                    // is this snake running into the other snake or itself? -> pepsi
                    if (SnakeCollision(snake.HeadPosition))
                    {
                        KillSnake(snake);
                        // everyone pepsi? game over?!
                        if (!_snakes[0].Alive && !_snakes[1].Alive)
                        {
                            _running = false;
                            AsciiArt.GameOver();
                            Console.ReadKey();
                        }
                            
                        continue;
                    }

                    // is this snake picking up food?
                    if (Collision(_food, snake.HeadPosition))
                    {
                        SpawnFood();
                    }
                    else
                    {
                        DeleteAt(snake.Segments.First());
                        snake.Segments.Remove(snake.Segments.First());
                    }

                    GrowSnake(snake);
                }

            }
        }

        private static void ProcessMovement()
        {
            foreach(Snake snake in _snakes)
            {
                Position2D position = snake.HeadPosition;

                switch (snake.Direction)
                {
                    case Direction.Up:
                        if (position.Y - 1 < _playArea.TopEdge)
                            position.Y = _playArea.BottomEdge;
                        else
                            --position.Y;
                        break;
                    case Direction.Down:
                        if (position.Y + 1 > _playArea.BottomEdge)
                            position.Y = _playArea.TopEdge;
                        else
                            ++position.Y;
                        break;
                    case Direction.Left:
                        if (position.X - 1 < _playArea.LeftEdge)
                            position.X = _playArea.RightEdge;
                        else
                            --position.X;
                        break;
                    case Direction.Right:
                        if (position.X + 1 > _playArea.RightEdge)
                            position.X = _playArea.LeftEdge;
                        else
                            ++position.X;
                        break;
                }

                snake.HeadPosition = position;
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
                        if(_snakes[0].Direction != Direction.Down)
                            _snakes[0].Direction = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        if (_snakes[0].Direction != Direction.Up)
                            _snakes[0].Direction = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (_snakes[0].Direction != Direction.Right)
                            _snakes[0].Direction = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (_snakes[0].Direction != Direction.Left)
                            _snakes[0].Direction = Direction.Right;
                        break;

                    // Movement P2
                    case ConsoleKey.W:
                        if (_snakes[1].Direction != Direction.Down)
                            _snakes[1].Direction = Direction.Up;
                        break;
                    case ConsoleKey.S:
                        if (_snakes[1].Direction != Direction.Up)
                            _snakes[1].Direction = Direction.Down;
                        break;
                    case ConsoleKey.A:
                        if (_snakes[1].Direction != Direction.Right)
                            _snakes[1].Direction = Direction.Left;
                        break;
                    case ConsoleKey.D:
                        if (_snakes[1].Direction != Direction.Left)
                            _snakes[1].Direction = Direction.Right;
                        break;

                    case ConsoleKey.Escape:
                        _running = false;
                        break;
                }
            }
        }
    }
}