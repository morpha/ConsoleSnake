using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSnake
{
    internal static class AsciiArt
    {
        public enum TextAlignment
        {
            Left,
            Centered,
            Right,
            Top,
            Bottom
        }
        private static (Int32 left, Int32 top) FindStartingPosition(Int32 numLines, Int32 numColumns, TextAlignment horizontalAlignment, TextAlignment verticalAlignment, int offsetLeft = 0, int offsetTop = 0)
        {
            Int32 left;
            Int32 top;

            switch(horizontalAlignment)
            {
                default:
                case TextAlignment.Left:
                    left = 0;
                    break;
                case TextAlignment.Centered:
                    left = Console.WindowWidth / 2 - numColumns / 2;
                    break;
                case TextAlignment.Right:
                    left = Console.WindowWidth - numColumns;
                    break;
            }
            
            switch(verticalAlignment)
            {
                default:
                case TextAlignment.Top:
                    top = 0;
                    break;
                case TextAlignment.Centered:
                    top = Console.WindowHeight / 2 - numLines / 2;
                    break;
                case TextAlignment.Bottom:
                    top = Console.WindowHeight - numLines;
                    break;
            }

            return (left + offsetLeft, top + offsetTop);
        }

        private static void DrawAt(ref string[] str, TextAlignment horizontalAlignment, TextAlignment verticalAlignment, ConsoleColor col = ConsoleColor.White, int offsetLeft = 0, int offsetTop = 0)
        {
            (Int32 left, Int32 top) startingPos = FindStartingPosition(str.Count(), str[0].Length, horizontalAlignment, verticalAlignment, offsetLeft, offsetTop);

            Console.SetCursorPosition(startingPos.left, startingPos.top);
            Console.ForegroundColor = col;
            
            foreach(var item in str)
            {
                Console.WriteLine(item);
                Console.CursorLeft = startingPos.left;
            }
        }

        public static void IntroAnimation()
        {
            bool loopRunning = true;

            (ConsoleColor col, Int32 sleepFor)[] animFrames =
            {
                (ConsoleColor.DarkGray, 150),
                (ConsoleColor.Gray, 100),
                (ConsoleColor.White, 170),
                (ConsoleColor.Gray, 90),
                (ConsoleColor.DarkGray, 190),
                (ConsoleColor.White, 40),
                (ConsoleColor.DarkGray, 100),
                (ConsoleColor.Gray, 160),
                (ConsoleColor.DarkGray, 100),
                (ConsoleColor.DarkGray, 60),
                (ConsoleColor.Gray, 100),
                (ConsoleColor.DarkGray, 40),
            };

            while (loopRunning)
            {
                foreach ((ConsoleColor col, Int32 sleepFor) frame in animFrames)
                {
                    CNake(TextAlignment.Centered, TextAlignment.Top, frame.col, offsetTop: 4);
                    Thread.Sleep(frame.sleepFor);
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey();
                        loopRunning = false;
                    }
                }
            }

            Console.Clear();
        }

        public static void CNake(TextAlignment horizontalAlignment = TextAlignment.Centered, TextAlignment verticalAlignment = TextAlignment.Centered, ConsoleColor col = ConsoleColor.White, Int32 offsetLeft = 0,  Int32 offsetTop = 0)
        {
            string[] lines =
            {
                @"      _____  _____   ______          ____    ____    ____       ______   ",
                @"  ___|\    \|\    \ |\     \    ____|\   \  |    |  |    |  ___|\     \  ",
                @" /    /\    \ \    \| \     \  /    /\    \ |    |  |    | |     \     \ ",
                @"|    |  |    |\|    \  \     ||    |  |    ||    | /    // |     ,_____/|",
                @"|    |  |____| |     \  |    ||    |__|    ||    |/ _ _//  |     \--'\_|/",
                @"|    |   ____  |      \ |    ||    .--.    ||    |\    \'  |     /___/|  ",
                @"|    |  |    | |    |\ \|    ||    |  |    ||    | \    \  |     \____|\ ",
                @"|\ ___\/    /| |____||\_____/||____|  |____||____|  \____\ |____ '     /|",
                @"| |   /____/ | |    |/ \|   |||    |  |    ||    |   |    ||    /_____/ |",
                @" \|___|    | / |____|   |___|/|____|  |____||____|   |____||____|     | /",
                @"   \( |____|/    \(       )/    \(      )/    \(       )/    \( |_____|/ ",
                @"    '   )/        '       '      '      '      '       '      '    )/    ",
                @"        '                                                          '     ",
                @"                                                                         ",
                @"                               PRESS ANY KEY                             "


            };
            DrawAt(ref lines, horizontalAlignment, verticalAlignment, col, offsetLeft, offsetTop);
        }
        
        public static void GameOver(TextAlignment horizontalAlignment = TextAlignment.Centered, TextAlignment verticalAlignment = TextAlignment.Centered, ConsoleColor col = ConsoleColor.White, Int32 offsetLeft = 0,  Int32 offsetTop = 0)
        {
            string[] lines =
            {
                @"   ___                   ___              ",
                @"  / __|__ _ _ __  ___   / _ \__ _____ _ _ ",
                @" | (_ / _` | '  \/ -_) | (_) \ V / -_) '_|",
                @"  \___\__,_|_|_|_\___|  \___/ \_/\___|_|  ",
                @"                                          ",
                @"                                          ",
                @"               PRESS ANY KEY              "


            };
            DrawAt(ref lines, horizontalAlignment, verticalAlignment, col, offsetLeft, offsetTop);
        }
        
        public static void Countdown(TextAlignment horizontalAlignment = TextAlignment.Centered, TextAlignment verticalAlignment = TextAlignment.Centered, ConsoleColor col = ConsoleColor.White, Int32 offsetLeft = 0,  Int32 offsetTop = 0)
        {
            string[][] numbers =
            {
                new string[] {
                    @"  ____",
                    @" |__ /",
                    @"  |_ \",
                    @" |___/",
                    @"      "
                },
                new string[]
                {
                    @"  ___ ",
                    @" |_  )",
                    @"  / / ",
                    @" /___|",
                    @"      "
                },
                new string[]
                {
                    @"   _  ",
                    @"  / | ",
                    @"  | | ",
                    @"  |_| ",
                    @"      "
                }
            };

            foreach(string[] lines in numbers) 
            {
                string[] curLines = lines;
                DrawAt(ref curLines, horizontalAlignment, verticalAlignment, col, offsetLeft, offsetTop);
                Thread.Sleep(1000);
            }

            string[] clearArea = new string[]
            {
                @"      ",
                @"      ",
                @"      ",
                @"      ",
                @"      "
            };
            DrawAt(ref clearArea, horizontalAlignment, verticalAlignment, col, offsetLeft, offsetTop);
        }

        
    }

    
}
