using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC = ConsoleCompanion;

namespace ConsoleSnake
{
    internal class Scoreboard
    {
        public static void SetScore(Int32 player, Int32 score)
        {
            CC.Write(score.ToString(), 2, 2 + player);
        }
    }
}
