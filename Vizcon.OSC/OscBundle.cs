using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vizcon.OSC
{
    public class OscBundle : OscPacket
    {
        Timetag _timetag;

        public ulong Timetag
        {
            get { return _timetag.Tag; }
            set { _timetag.Tag = value; }
        }

        public DateTime Timestamp
        {
            get { return _timetag.Timestamp; }
            set { _timetag.Timestamp = value; }
        }

        public List<OscMessage> Messages;

        public OscBundle(DateTime timestamp, params OscMessage[] args)
        {
            _timetag = new Timetag(DateTimeToOscTimetag(timestamp));
            Messages = [.. args];
        }

        private static ulong DateTimeToOscTimetag(DateTime timestamp)
        {
            DateTime oscEpoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = timestamp.ToUniversalTime() - oscEpoch;

            uint seconds = (uint)diff.TotalSeconds;
            uint fractional = (uint)((diff.TotalSeconds - seconds) * 4294967296.0); // Convert fractional part

            return ((ulong)seconds << 32) | fractional;
        }

        public override byte[] GetBytes()
        {
            string bundle = "#bundle";
            int bundleTagLen = Utils.AlignedStringLength(bundle);
            byte[] tag = SetULong(_timetag.Tag);

            List<byte[]> outMessages = new();
            foreach (OscMessage msg in Messages)
            {
                outMessages.Add(msg.GetBytes());
            }

            int len = bundleTagLen + tag.Length + outMessages.Sum(x => x.Length + 4);

            int i = 0;
            byte[] output = new byte[len];
            Encoding.ASCII.GetBytes(bundle).CopyTo(output, i);
            i += bundleTagLen;
            tag.CopyTo(output, i);
            i += tag.Length;

            foreach (byte[] msg in outMessages)
            {
                var size = SetInt(msg.Length);
                size.CopyTo(output, i);
                i += size.Length;

                msg.CopyTo(output, i);
                i += msg.Length;
            }

            return output;
        }

    }
}
