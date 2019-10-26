using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUI.Utilities
{
    public enum ConsoleTableBorder
    {
        OneLine,
        TwoLines
    }

    public enum TableTextAlign
    {
        Left,
        Right,
        Center
    }

    public class ConsoleTable
    {
        private readonly Dictionary<string, char> OneLineBorderElements = new Dictionary<string, char>
        {
            {"lu", '┌'},
            {"ru", '┐'},
            {"lb", '└'},
            {"rb", '┘'},
            {"h", '─'},
            {"v", '│'},
            {"hd", '┬'},
            {"hu", '┴'},
            {"vr", '├'},
            {"vl", '┤'},
            {"hv", '┼'}
        };

        private readonly Dictionary<string, char> TwoLinesBorderElements = new Dictionary<string, char>
        {
            {"lu", '╔'},
            {"ru", '╗'},
            {"lb", '╚'},
            {"rb", '╝'},
            {"h", '═'},
            {"v", '║'},
            {"hd", '╦'},
            {"hu", '╩'},
            {"vr", '╠'},
            {"vl", '╣'},
            {"hv", '╬'}
        };

        private Dictionary<string, char> _border;
        private ConsoleTableBorder _bt;
        private readonly List<List<string>> _table = new List<List<string>>();


        public ConsoleTable(ConsoleTableBorder border = ConsoleTableBorder.OneLine,
            TableTextAlign al = TableTextAlign.Center)
        {
            BorderType = border;
            Align = al;
        }

        public TableTextAlign Align { get; set; }

        public ConsoleTableBorder BorderType
        {
            get => _bt;
            set
            {
                switch (value)
                {
                    case ConsoleTableBorder.OneLine:
                        _border = OneLineBorderElements;
                        break;
                    case ConsoleTableBorder.TwoLines:
                        _border = TwoLinesBorderElements;
                        break;
                    default:
                        throw new Exception("Border type can be OneLine or TwoLines only");
                }
                _bt = value;
            }
        }

        public void AddRow(List<string> columns)
        {
            _table.Add(columns);
        }

        public void RemoveRow(int index)
        {
            _table.RemoveAt(index);
        }

        private string al(string inp, int l)
        {
            switch (Align)
            {
                case TableTextAlign.Right:
                    return string.Format("{0," + l + "}", inp);
                case TableTextAlign.Left:
                    return string.Format("{0,-" + l + "}", inp);
                case TableTextAlign.Center:
                    return string.Format("{0,-" + l + "}",
                        string.Format("{0," + (l + inp.Length) / 2 + "}", inp));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string GetTable()
        {
            if (_table.Count == 0) return string.Empty;
            var maxW = new List<int>();
            for (var i = 0; i < _table.Select(r => r.Count).Max(); i++)
                maxW.Add(0);
            var res = _border["lu"].ToString();
            foreach (var row in _table)
                for (var i = 0; i < row.Count; i++)
                    if (maxW[i] < row[i].Length)
                        maxW[i] = row[i].Length;
            for (var i = 0; i < maxW.Count; i++)
            {
                res += new string(_border["h"], maxW[i]);
                res += i != maxW.Count - 1 ? _border["hd"] : _border["ru"];
            }
            res += "\n";
            for (var i = 0; i < _table.Count; i++)
            {
                for (var j = 0; j < _table[i].Count; j++)
                    res += $"{_border["v"]}{al(_table[i][j], maxW[j])}";
                res += _border["v"] + "\n";
                if (i != _table.Count - 1)
                {
                    res += _border["vr"];
                    for (var j = 0; j < maxW.Count; j++)
                    {
                        res += new string(_border["h"], maxW[j]);
                        res += j != maxW.Count - 1 ? _border["hv"] : _border["vl"];
                    }
                    res += "\n";
                }
                else
                {
                    res += _border["lb"];
                    for (var j = 0; j < maxW.Count; j++)
                    {
                        res += new string(_border["h"], maxW[j]);
                        res += j != maxW.Count - 1 ? _border["hu"] : _border["rb"];
                    }
                }
            }
            return res;
        }
    }
}