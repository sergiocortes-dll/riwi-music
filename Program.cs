using System;
using System.Globalization;
using riwi_music.Models;

namespace riwi_music
{
    public class Program
    {
        public static DateTime BuildDate(int? year = null, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null)
        {
            int y = year ?? DateTime.MinValue.Year;
            int m = month ?? DateTime.MinValue.Month;
            int d = day ?? DateTime.MinValue.Day;
            int h = hour ?? 0;
            int ms = minute ?? 0;
            int ds = second ?? 0;
            return new DateTime(y, m, d, h, ms, ds);
        }

        
        public static void Main(string[] args)
        {
            Console.WriteLine(BuildDate().ToString("F"));
            Console.WriteLine(BuildDate(2025, 12, 25, 23, 20, 10).ToString(""));
        }
    }
}