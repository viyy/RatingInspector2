using System;
using System.Collections.Generic;

namespace ConsoleUI.Utilities
{
    public static class ConsoleMenu
    {
        public static int Show(List<string> menuItemsList)
        {
            var active = 0;
            ConsoleKey k;
            Console.CursorVisible = false;
            do
            {
                Console.Clear();
                for (var i = 0; i < menuItemsList.Count; i++)
                    Console.WriteLine((i == active ? "[>]" : "[ ]") + menuItemsList[i]);
                Console.WriteLine("\nUse <arrow up> and <arrow down> to navigate.\nUse <enter> to confirm.");
                k = Console.ReadKey().Key;
                switch (k)
                {
                    case ConsoleKey.DownArrow:
                        active++;
                        break;
                    case ConsoleKey.UpArrow:
                        active--;
                        break;
                }
                if (active == -1) active = menuItemsList.Count - 1;
                if (active == menuItemsList.Count) active = 0;
            } while (k != ConsoleKey.Enter);
            Console.CursorVisible = true;
            return active;
        }
    }
}