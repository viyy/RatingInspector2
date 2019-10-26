using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsoleUI.Utilities;
using Interfaces;

namespace ConsoleUI.Pages
{
    public class UpdatePage : ScreenModel
    {
        private IUpdateService _update;
        private static List<string> _status = new List<string>();
        private static bool _busy = false;

        public UpdatePage(IUpdateService update)
        {
            _update = update;
            Title = "RI2: Update";
            PageGuid = 100;
            Id = 1;
            MenuButton = "Update";
        }
        public override bool Render()
        {   
            if (!_busy)
                Console.WriteLine("[Enter] to update db");
            foreach (var s in _status)
            {
                Console.WriteLine(s);
            }
            return false;
        }

        public override void Enter()
        {
            if (_busy) return;
            _busy = true;
            var pr = new Progress<string>(s=>
            {
                _status.Add(s);
                if (Active) Console.WriteLine(s);
            });
            var t = Task.Run(async () =>
            {
                _status.Clear();
                _status.Add("Updating...");
                if (Active) Console.WriteLine("Updating...");
                await _update.UpdateAsync(pr); 
                if (Active) Console.WriteLine("Updated!");
                _status.Clear();
                _status.Add($"Last update: {DateTime.Now.ToLongDateString()}");
                _busy = false;
                EventManager.Emit("dbupdate", EventArgs.Empty);
            });
        }
    }
}