﻿using System;
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

        public OscBundle(ulong timetag, params OscMessage[] args)
        {
            _timetag = new Timetag(timetag);
            Messages = new List<OscMessage>();
            Messages.AddRange(args);
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
