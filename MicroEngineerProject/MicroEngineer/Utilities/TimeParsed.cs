namespace MicroMod
{
    public struct TimeParsed
    {
        public int Years;
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;

        public static TimeParsed MaxValue() => new TimeParsed { Years = int.MaxValue, Days = 425, Hours = 5, Minutes = 59, Seconds = 59 };
        public static TimeParsed MinValue() => new TimeParsed { Years = -int.MaxValue, Days = 425, Hours = 5, Minutes = 59, Seconds = 59 };
    }
}
