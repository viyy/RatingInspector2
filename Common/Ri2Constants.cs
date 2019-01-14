namespace Common
{
    public static class Ri2Constants
    {
        public const string TmpPath = @"tmp";
        public const string FideZipPath = @"tmp\fide.zip";
        public const string RcfFilePath = @"tmp\rcf.xlsx";
        public const string FideFilePath = @"tmp\players_list_xml_foa.xml";

        public static class Notifications
        {
            public const string DbUpdated = "db_upd";
            public const string Exit = "app_exit";
        }

        public static class FideXmlElements
        {
            public const string Root = "players";
            public const string Player = "player";
            public const string FideId = "fideid";
            public const string Country = "country";
            public const string Name = "name";
            public const string Std = "rating";
            public const string Rpd = "rapid_rating";
            public const string Blz = "blitz_rating";
            public const string Birth = "birthday";
        }

        public static class RcfColumns
        {
            public const int RcfId = 1;
            public const int FideId = 2;
            public const int Name = 4;
            public const int Birth = 6;
            public const int Std = 7;
            public const int Rpd = 9;
            public const int Blz = 11;
        }
    }
}