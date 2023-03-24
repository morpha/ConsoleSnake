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

        private static Direction _direction = Direction.Up;
        static void Main(string[] args)
        {
            Console.SetCursorPosition(Console.WindowWidth/2, Console.WindowHeight/2);
            for(int i = 1; ;++i)
            {
                HandleInput();
                ProcessMovement();
                DrawSnake();
                Thread.Sleep(100);
            }
        }

        private static void DrawSnake()
        {
            Console.Write('#');
            --Console.CursorLeft;
        }

        private static void ProcessMovement()
        {
            switch (_direction)
            {
                case Direction.Up:
                    --Console.CursorTop;
                    break;
                case Direction.Down:
                    ++Console.CursorTop;
                    break;
                case Direction.Right:
                    ++Console.CursorLeft;
                    break;
                case Direction.Left:
                    --Console.CursorLeft;
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
                        _direction = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        _direction = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        _direction = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        _direction = Direction.Right;
                        break;
                }
            }
        }
    }
}