namespace Vizcon.OSC
{
    public struct RGBA
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public RGBA(byte red, byte green, byte blue, byte alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj?.GetType() == typeof(RGBA))
            {
                if (R == ((RGBA)obj).R && G == ((RGBA)obj).G && B == ((RGBA)obj).B && A == ((RGBA)obj).A)
                    return true;
                else
                    return false;
            }
            else if (obj?.GetType() == typeof(byte[]))
            {
                if (R == ((byte[])obj)[0] && G == ((byte[])obj)[1] && B == ((byte[])obj)[2] && A == ((byte[])obj)[3])
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static bool operator ==(RGBA a, RGBA b)
        {
            if (a.Equals(b))
                return true;
            else
                return false;
        }

        public static bool operator !=(RGBA a, RGBA b)
        {
            if (!a.Equals(b))
                return true;
            else
                return false;
        }

        public override readonly int GetHashCode()
        {
            return (R << 24) + (G << 16) + (B << 8) + (A);
        }
    }
}
