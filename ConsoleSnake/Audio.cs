using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSnake
{
    static class Audio
    {
        public static void GameOver()
        {
            Console.Beep(800, 200);
            Console.Beep(700, 150);
            Console.Beep(500, 100);
            Console.Beep(300, 100);
        }
    }
}
