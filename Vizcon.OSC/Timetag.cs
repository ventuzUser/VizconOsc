using System;

namespace Vizcon.OSC
{
    public struct Timetag
    {
        public ulong Tag;

        public DateTime Timestamp
        {
            readonly get => Utils.TimetagToDateTime(Tag);
            set
            {
                Tag = Utils.DateTimeToTimetag(value);
            }
        }

        public double Fraction
        {
            readonly get => Utils.TimetagToFraction(Tag);
            set
            {
                Tag = (Tag & 0xFFFFFFFF00000000) + (UInt32)(value * 0xFFFFFFFF);
            }
        }

        public Timetag(ulong value)
        {
            Tag = value;
        }

        public Timetag(DateTime value)
        {
            Tag = 0;
            Timestamp = value;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Timetag))
            {
                if (Tag == ((Timetag)obj).Tag)
                    return true;
                else
                    return false;
            }
            else if (obj.GetType() == typeof(ulong))
            {
                if (Tag == ((ulong)obj))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static bool operator ==(Timetag a, Timetag b)
        {
            if (a.Equals(b))
                return true;
            else
                return false;
        }

        public static bool operator !=(Timetag a, Timetag b)
        {
            if (a.Equals(b))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (int)(((uint)(Tag >> 32) + (uint)(Tag & 0x00000000FFFFFFFF)) / 2);
        }
    }
}
