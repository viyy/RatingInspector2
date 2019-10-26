using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Common
{
    public class Settings
    {
        private static Settings _current;

        private Settings()
        {
        }

        public static Settings Current => _current ?? (_current = Load());

        public string FideUrl { get; set; }
        public List<string> Filter { get; set; }
        public string RcfUrl { get; set; }
        public int BirthCutoff { get; set; }
        public bool RemoveTmpFiles { get; set; }

        public void Reload()
        {
            _current = Load();
        }

        private static Settings Load()
        {
            try
            {
                if (!File.Exists("Settings.xml")) Init();
                var serializer = new XmlSerializer(typeof(Settings), new XmlRootAttribute("settings"));
                using (var f = new StreamReader("Settings.xml"))
                {
                    var tmp = (Settings) serializer.Deserialize(f);
                    return tmp;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Settings", $"Error while loading: {ex.Message}", LogLevel.Error);
            }
            return new Settings
            {
                FideUrl = "http://ratings.fide.com/download/players_list_xml.zip",
                Filter = new List<string> {"RUS"},
                RcfUrl = "https://goo.gl/g5wsY3",
                RemoveTmpFiles = true,
                BirthCutoff = 1990
            };
        }

        public void Save()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Settings), new XmlRootAttribute("settings"));
                using (var f = new StreamWriter("Settings.xml"))
                {
                    serializer.Serialize(f, this);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Settings", $"Error while saving: {ex.Message}", LogLevel.Error);
            }
        }

        private static void Init()
        {
            try
            {
                var tmp = new Settings
                {
                    FideUrl = "http://ratings.fide.com/download/players_list_xml.zip",
                    Filter = new List<string> {"RUS"},
                    RcfUrl = "https://goo.gl/g5wsY3",
                    RemoveTmpFiles = true,
                    BirthCutoff = 1990
                };
                var serializer = new XmlSerializer(typeof(Settings), new XmlRootAttribute("settings"));
                using (var f = new StreamWriter("Settings.xml"))
                {
                    serializer.Serialize(f, tmp);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Settings", $"Error while initializing: {ex.Message}", LogLevel.Error);
            }
        }
    }
}