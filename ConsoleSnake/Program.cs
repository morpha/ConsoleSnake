using System.Data;
using System.Xml.Linq;
using CC = ConsoleCompanion;

namespace ConsoleSnake
{
    internal class Program
    {
        public enum GameMode
        {
            Singleplayer,
            Multiplayer
        }

        const Int32 MINWIDTH = 82;
        const Int32 MINHEIGHT = 35;

        private static PlayArea _playArea = new(1, 4, Console.WindowWidth - 2, Console.WindowHeight - 2);

        private static bool _gameRunning = true;
        private static bool _running = true;

        private static GameMode _mode = GameMode.Singleplayer;

        private static Snake[] _snakes = {
            new Snake(),
            new Snake()
        };
        private static Position2D _food = new(-1, -1);
        private static Random rng = new();

        private static Int32 _windowWidth = MINWIDTH;
        private static Int32 _windowHeight = MINHEIGHT;

        private static Int32 _desiredFrameTime = 50;

        private static DateTime _frameTiming = DateTime.Now;

        static void Setup()
        {
            CC.Resize(_windowWidth, _windowHeight);
            Console.CursorVisible = false;

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            _playArea = new PlayArea(1, 4, Console.WindowWidth - 2, Console.WindowHeight - 2);
        }
        static void Main(string[] args)
        {
            Setup();
            Intro();
            MainLoop();
        }
        static void Intro()
        {
            AsciiArt.IntroAnimation();
        }

        static void Options()
        {
            Menu menu = new();
            menu.AddItem("Window Size", "Play a single-player game.", () => { _windowWidth = Console.WindowWidth; _windowHeight = Console.WindowHeight; Setup(); return true; });
            menu.AddItem("Some Option 2", "Play a two-player game.", () => { return true; });
            menu.AddItem("Some Option 3", "Change game options.", () => { return true; });
            menu.AddItem("Back", "Return to the menu.", () => { return false; });
            menu.Show();
        }

        private static void ShowMenu()
        {
            Menu menu = new();
            menu.AddItem("Play [1P]", "Play a single-player game.", () => { _mode = GameMode.Singleplayer;  return false; });
            menu.AddItem("Play [2P]", "Play a two-player game.", () => { _mode = GameMode.Multiplayer; return false; });
            menu.AddItem("Options", "Change game options.", () => { Options(); return true; });
            menu.AddItem("Quit", "Quit the game.", () => { _gameRunning = false;  return false; });
            menu.Show();
        }

        public static bool WindowIntegrity(Action postfix)
        {
            bool tainted = false ;
            try
            {
                if (tainted = WindowIntegrity())
                    postfix();
            }
            catch { }
            return tainted;
        }

        /// <summary>
        /// Checks whether the window size has changed, changes it to at least the minimum set via MINXXX constants if needed and updates the internal bounds.
        /// </summary>
        /// <returns>true if the window size has changed and the screen buffer has been tainted.</returns>
        public static bool WindowIntegrity()
        {
            bool tainted = false;
            if (tainted = Console.WindowHeight != _windowHeight)
            {
                _windowHeight = (Console.WindowHeight < MINHEIGHT ? MINHEIGHT : Console.WindowHeight);
            }
            if (Console.WindowWidth != _windowWidth)
            {
                tainted = true;
                _windowWidth = (Console.WindowWidth < MINWIDTH ? MINWIDTH : Console.WindowWidth);
            }

            if (tainted)
            {
                Console.CursorVisible = false;
                CC.Resize(_windowWidth, _windowHeight);
            }

            return tainted;
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
                try
                {
                    WindowIntegrity(Setup);
                    Console.Clear();
                    AsciiArt.CNake(AsciiArt.TextAlignment.Centered, AsciiArt.TextAlignment.Top, offsetTop: 1);
                    ShowMenu();
                    if (!_gameRunning)
                        break;
                    Console.Clear();
                    Reset();
                    DrawPlayArea();
                    AsciiArt.Countdown(col: ConsoleColor.Magenta);
                    SpawnFood();

                    while (_running)
                    {
                        if (WindowIntegrity())
                        {
                        }
                        TimeFrame();
                        HandleInput();
                        AllSnakeThings();
                    }
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    WindowIntegrity(Setup);
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
            _running = true;

            _snakes[0].Direction = Direction.Right;
            _snakes[1].Direction = Direction.Left;
            _snakes[0].Color = ConsoleColor.Green;
            _snakes[1].Color = ConsoleColor.Cyan;
            _snakes[0].Segments.Clear();
            _snakes[1].Segments.Clear();
            _snakes[0].Alive = true;
            _snakes[1].Alive = _mode == GameMode.Multiplayer;

            // Set up anew
            _snakes[0].Segments.Add(_snakes[0].HeadPosition = new Position2D(CC.CenterX, CC.CenterY));
            if(_mode == GameMode.Multiplayer)
                _snakes[1].Segments.Add(_snakes[1].HeadPosition = new Position2D(CC.CenterX - 1, CC.CenterY));
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
            CC.CursorPosition = (x, y);
            Console.ForegroundColor = color;
            Console.Write(text);
        }

        private static void GrowSnake(Snake snake)
        {
            ++snake;
            WriteColAt('#', snake.Color, snake.HeadPosition);
        }

        private static void DeleteAt(Position2D pos) => DeleteAt(pos.X, pos.Y);
        private static void DeleteAt(Int32 x, Int32 y)
        {
            CC.CursorPosition = (x, y);
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
            snake.Kill();
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
                            GameOver();
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

        private static void GameOver()
        {
            _running = false;
            AsciiArt.GameOver();
            Console.Beep(800, 200);
            Console.Beep(700, 150);
            Console.Beep(500, 100);
            Console.Beep(300, 100);
            Console.ReadKey();
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