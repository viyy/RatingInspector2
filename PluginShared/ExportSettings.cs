namespace Common
{
    public class ExportSettings
    {
        public bool Rcf { get; set; } = true;
        public bool Fide { get; set; } = true;
        public bool RuName { get; set; } = true;
        public bool EngName { get; set; } = false;
        public bool Birth { get; set; } = true;
        public bool Groups { get; set; } = false;
        public bool[] RcfRat { get; set; } = {true, true, true};
        public bool[] FideRat { get; set; } = {true, true, true};
    }
}