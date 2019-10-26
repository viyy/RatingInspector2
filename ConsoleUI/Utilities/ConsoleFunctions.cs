//6. * Создать класс с методами, которые могут пригодиться в вашей учебе(Print, Pause). Lesson 1.
//Кожин Виктор

using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI.Utilities
{
    public static class ConsoleFunctions
    {
        /// <summary>
        ///     Prints the message from (x,y) position
        /// </summary>
        /// <param name="msg">Text</param>
        /// <param name="x">Position from left</param>
        /// <param name="y">Position from top</param>
        public static void PrintAt(string msg, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(msg);
        }

        /// <summary>
        ///     Prints the message in the center of screen
        /// </summary>
        /// <param name="msg">Text</param>
        public static void PrintAtCenter(string msg)
        {
            var x = (Console.WindowWidth - msg.Length) / 2;
            var y = Console.WindowHeight / 2;
            PrintAt(msg, x, y);
        }

        /// <summary>
        ///     Pause console app until user press a key
        /// </summary>
        public static void Pause()
        {
            Console.ReadKey();
        }

        /// <summary>
        ///     Pause console app until user press a key
        /// </summary>
        /// <param name="msg">Information message</param>
        public static void Pause(string msg)
        {
            Console.Write(msg);
            Console.ReadKey();
        }

        /// <summary>
        ///     Read line from console and convert it to double
        /// </summary>
        /// <param name="promt">Message before read</param>
        /// <returns>Entered number</returns>
        public static double GetDouble(string promt = null)
        {
            if (promt != null)
                Console.Write(promt);
            return Convert.ToDouble(Console.ReadLine());
        }

        /// <summary>
        ///     Read line from console and convert it to Int32
        /// </summary>
        /// <param name="promt">Message before read</param>
        /// <returns>Entered number</returns>
        public static int GetInt(string promt = null)
        {
            if (promt != null)
                Console.Write(promt);
            return Convert.ToInt32(Console.ReadLine());
        }

        /// <summary>
        ///     Returns string user entered, but hide it in console
        /// </summary>
        /// <returns></returns>
        public static string GetInvisibleString()
        {
            var input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }
        
        
        private static Dictionary<string, (ConsoleColor, ConsoleColor)> _styles = new Dictionary<string, (ConsoleColor, ConsoleColor)>
        {
            {"active", (ConsoleColor.Black, ConsoleColor.White)},
            {"normal", (ConsoleColor.White, ConsoleColor.Black)}
        };

        public static void RegisterStyle(string name, ConsoleColor foreground, ConsoleColor background)
        {
            if (_styles.ContainsKey(name))
            {
                _styles[name] = (foreground, background);
            }
            else
            {
                _styles.Add(name, (foreground, background));
            }
        }

        public static void ResetStyles()
        {
            _styles = new Dictionary<string, (ConsoleColor, ConsoleColor)>
            {
                {"active", (ConsoleColor.Black, ConsoleColor.White)},
                {"normal", (ConsoleColor.White, ConsoleColor.Black)}
            };
            SetStyle("normal");
        }

        public static void SetStyle(string name)
        {
            var (fore, back) = _styles.ContainsKey(name) ? _styles[name] : _styles["normal"];
            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
        }
    }
}