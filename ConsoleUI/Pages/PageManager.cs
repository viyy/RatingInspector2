using System;
using System.Collections.Generic;
using System.Linq;
using CF = ConsoleUI.Utilities.ConsoleFunctions;

namespace ConsoleUI.Pages
{
    public class PageManager
    {
        private List<ScreenModel> _pages = new List<ScreenModel>();

        private ScreenModel _current = null;
        private int _currentId;

        public void RegisterPage(ScreenModel page)
        {
            if (_pages.Any(x=>x.PageGuid==page.PageGuid)) return;
            _pages.Add(page);
        }

        public void Init()
        {
            _pages = _pages.OrderBy(x=>x.Id).ToList();
            if (_current == null)
                _current = _pages.FirstOrDefault();
            if (_current == null)
                throw new Exception("Register at least one page");
            _currentId = 0;
            _current.Active = true;
        }

        public bool Render()
        {
            _current.Active = false;
            Console.Clear();
            CF.SetStyle("normal");
            //Menu
            foreach (var page in _pages)
            {
                if (_current != page)
                {
                    Console.Write($"[{page.MenuButton}]");
                }
                else
                {
                    CF.SetStyle("active");
                    Console.Write($"[{page.MenuButton}]");
                    CF.SetStyle("normal");
                }
            }
            //Page
            Console.WriteLine();
            var str = new string('═', (Console.WindowWidth-_current.Title.Length-2)/2) 
                      + " " + _current.Title + " "
                      + new string('═', (Console.WindowWidth-_current.Title.Length-2)/2);
            Console.WriteLine(str);
            _current.Active = true;
            var skipInput = _current.Render();
            if (skipInput) return false;
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.Tab:
                {
                    _current.Active = false;
                    _currentId++;
                    if (_currentId == _pages.Count)
                        _currentId = 0;
                    _current = _pages[_currentId];
                    _current.Active = true;
                    return false;
                }

                case ConsoleKey.Escape:
                    return true;
                
                case ConsoleKey.Enter:
                    _current.Enter();
                    return false;
                default:
                    _current.Input(key);
                    return false;
            }
        }
    }
}