namespace Vizcon.OSC
{
    public class Symbol
    {
        public string Value;

        public Symbol()
        {
            Value = "";
        }

        public Symbol(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Symbol))
            {
                if (Value == ((Symbol)obj).Value)
                    return true;
                else
                    return false;
            }
            else if (obj.GetType() == typeof(string))
            {
                if (Value == ((string)obj))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static bool operator ==(Symbol a, Symbol b)
        {
            if (a.Equals(b))
                return true;
            else
                return false;
        }

        public static bool operator !=(Symbol a, Symbol b)
        {
            if (!a.Equals(b))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
