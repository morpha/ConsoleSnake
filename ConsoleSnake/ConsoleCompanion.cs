using ConsoleSnake;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

static class ConsoleCompanion
{
    /// <summary>Calculate and return the horizontal center of the console window.</summary>
    /// <returns>Horizontal center as Int32 in columns from the left.</returns>
    public static Int32 CenterX { get => Console.WindowWidth / 2; }
    /// <summary>Calculate and return the vertical center of the console window.</summary>
    /// <returns>Vertical center as Int32 in lines from the top.</returns>
    public static Int32 CenterY { get => Console.WindowHeight / 2; }
    /// <summary>Resize both the console window and its buffer, eliminating scroll bars.</summary>
    /// <param name="width">Desired width in columns as Int32.</param>
    /// <param name="height">Desired height in lines as Int32.</param>
    public static void Resize(Int32 width, Int32 height)
    {
        try
        {
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(Console.WindowLeft + Console.WindowWidth, Console.WindowTop + Console.WindowHeight);
        }
        catch { }
    }

    public static void Write(object text, Int32 x, Int32 y, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null) => Write(text.ToString() ?? "", x, y);
    public static void Write(string text, Int32 x, Int32 y, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        CursorPosition = (x, y);
        if(foregroundColor != null)
            Console.ForegroundColor = (ConsoleColor)foregroundColor;
        if (backgroundColor != null)
            Console.BackgroundColor = (ConsoleColor)backgroundColor;
        Console.Write(text);
    }

    /// <summary>
    /// Set or get the cursor position as a tuple (left, top).
    /// </summary>
    public static (Int32 left, Int32 top) CursorPosition { 
        get => (Console.CursorLeft, Console.CursorTop); 
        set {
            try { Console.SetCursorPosition(value.left, value.top); }
            catch { }
            Console.CursorVisible = false;
        }
    }
}
