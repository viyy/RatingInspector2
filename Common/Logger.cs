using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Common
{
    public static class Logger
    {
        public static void Log(string msg, LogLevel level = LogLevel.Info)
        {
            Log(SystemComponent, msg, level);
        }

        public static void Log(string component, string msg, LogLevel level = LogLevel.Info)
        {
            Log(component, new []{msg}, level);
        }
        
        public static void Log(IEnumerable<string> messages, LogLevel level = LogLevel.Info)
        {
            Log(SystemComponent, messages, level);
        }
        
        public static void Log(string component, IEnumerable<string> messages, LogLevel level = LogLevel.Info)
        {
            if (level >= _level)
                File.AppendAllLines(LogFile, messages.Select(s=>$"[{level}] [{DateTime.Now:G}] [{component}] {s}"));
        }
        
        public static void SetLevel(LogLevel level)
        {
            _level = level;
        }

        private static LogLevel _level = LogLevel.Error;

        private static readonly string LogFile = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["logfile"])
            ? ConfigurationManager.AppSettings["logfile"]
            : "log.txt";

        private const string SystemComponent = "RI2";

    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}