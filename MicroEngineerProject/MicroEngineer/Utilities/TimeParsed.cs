namespace MicroMod
{
    public struct TimeParsed
    {
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;

        public static TimeParsed MaxValue() => new TimeParsed { Days = int.MaxValue, Hours = 23, Minutes = 59, Seconds = 59 };
        public static TimeParsed MinValue() => new TimeParsed { Days = int.MinValue, Hours = 23, Minutes = 59, Seconds = 59 };
    }
}
