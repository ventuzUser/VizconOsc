namespace Vizcon.OSC
{
    public struct Midi
    {
        public byte Port;
        public byte Status;
        public byte Data1;
        public byte Data2;

        public Midi(byte port, byte status, byte data1, byte data2)
        {
            Port = port;
            Status = status;
            Data1 = data1;
            Data2 = data2;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj?.GetType() == typeof(Midi))
            {
                if (Port == ((Midi)obj).Port && Status == ((Midi)obj).Status && Data1 == ((Midi)obj).Data1 && Data2 == ((Midi)obj).Data2)
                    return true;
                else
                    return false;
            }
            else if (obj?.GetType() == typeof(byte[]))
            {
                if (Port == ((byte[])obj)[0] && Status == ((byte[])obj)[1] && Data1 == ((byte[])obj)[2] && Data2 == ((byte[])obj)[3])
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static bool operator ==(Midi a, Midi b)
        {
            if (a.Equals(b))
                return true;
            else
                return false;
        }

        public static bool operator !=(Midi a, Midi b)
        {
            if (!a.Equals(b))
                return true;
            else
                return false;
        }

        public override readonly int GetHashCode()
        {
            return (Port << 24) + (Status << 16) + (Data1 << 8) + (Data2);
        }
    }
}
