using System;

namespace Vizcon.OSC
{
    public class Utils
    {
        private static readonly DateTime Epoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a timetag to a DateTime.
        /// </summary>
        /// <param name="val">The timetag value.</param>
        /// <returns>The corresponding DateTime.</returns>
        public static DateTime TimetagToDateTime(UInt64 val)
        {
            if (val == 1)
                return DateTime.UtcNow;

            UInt32 seconds = (UInt32)(val >> 32);
            double fraction = TimetagToFraction(val);

            return Epoch.AddSeconds(seconds).AddSeconds(fraction);
        }

        /// <summary>
        /// Converts a timetag to a fractional second.
        /// </summary>
        /// <param name="val">The timetag value.</param>
        /// <returns>The fractional second.</returns>
        public static double TimetagToFraction(UInt64 val)
        {
            if (val == 1)
                return 0.0;

            UInt32 fractionBits = (UInt32)(val & 0xFFFFFFFF);
            return (double)fractionBits / 0xFFFFFFFF;
        }

        /// <summary>
        /// Converts a DateTime to a timetag.
        /// </summary>
        /// <param name="value">The DateTime value.</param>
        /// <returns>The corresponding timetag.</returns>
        public static UInt64 DateTimeToTimetag(DateTime value)
        {
            TimeSpan timeSpan = value.ToUniversalTime() - Epoch;
            UInt64 seconds = (UInt32)timeSpan.TotalSeconds;
            UInt64 fraction = (UInt32)(0xFFFFFFFF * (timeSpan.Milliseconds / 1000.0));

            return (seconds << 32) + fraction;
        }

        /// <summary>
        /// Calculates the aligned string length.
        /// </summary>
        /// <param name="val">The input string.</param>
        /// <returns>The aligned string length.</returns>
        public static int AlignedStringLength(string val)
        {
            int len = val.Length + 1; // +1 for null terminator
            while (len % 4 != 0) len++; // Ensure 4-byte alignment
            return len;
        }

    }
}
