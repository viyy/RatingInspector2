using System;
using System.Collections.Generic;
using ConsoleUI.Utilities;
using Interfaces;
using CF = ConsoleUI.Utilities.ConsoleFunctions;

namespace ConsoleUI.Pages
{
    public class MainPage : ScreenModel
    {
        private string _version = "v.1.0beta";
        private IInfo _info;
        private string _db;
        private string _pr;
        private string _rcf;
        private string _fide;
        private string _upd;
        
        public MainPage(IInfo info)
        {
            PageGuid = 1;
            Title = "Rating Inspector 2 Console";
            Id = 0;
            MenuButton = "Home";
            _info = info;
            UpdateData();
            EventManager.On("dbupdate", args => UpdateData());
        }

        private void UpdateData()
        {
            _db = _info.Version;
            _pr = _info.ProfilesCount.ToString();
            _rcf = _info.RcfCount.ToString();
            _fide = _info.FideCount.ToString();
            _upd = _info.LastUpdate.ToShortDateString();
        }
        public override bool Render()
        {
            var table = new ConsoleTable(ConsoleTableBorder.OneLine, TableTextAlign.Left);
            table.AddRow(new List<string>{"Version", _version});
            table.AddRow(new List<string>{"Db Version", _db});
            table.AddRow(new List<string>{"Profiles", _pr});
            table.AddRow(new List<string>{"Rcf", _rcf});
            table.AddRow(new List<string>{"Fide", _fide});
            table.AddRow(new List<string>{"Last update:", _upd});
            Console.Write(table.GetTable());
            return false;
        }
        
    }
}