namespace MicroMod
{
    public struct Stage
    {
        public int StageNumber { get; set; }
        public double DeltaVActual { get; set; }
        public float TwrActual { get; set; }
        public int BurnDays { get; set; }
        public int BurnHours { get; set; }
        public int BurnMinutes { get; set; }
        public int BurnSeconds { get; set; }
    }
}
