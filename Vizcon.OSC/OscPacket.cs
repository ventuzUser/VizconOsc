﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Vizcon.OSC
{
    public abstract class OscPacket
    {
        public static OscPacket GetPacket(byte[] OscData)
        {
            if (OscData == null || OscData.Length == 0)
                throw new ArgumentException("OSC data array is empty.", nameof(OscData));

            return OscData[0] == '#' ? parseBundle(OscData) : ParseMessage(OscData);
        }

        public abstract byte[] GetBytes();

        #region Parse OSC packages
        private static OscMessage ParseMessage(byte[] msg)
        {
            int index = 0;

            string address = GetAddress(msg, ref index);
            char[] types = GetTypes(msg, ref index);
            // Ensure index is correctly aligned before parsing arguments
            AlignIndex(index: ref index);
            Debug.WriteLine($"After Aligning Index for Arguments: {index}");

            List<object> arguments = [];
            List<object> mainArray = arguments; // used as a reference when we are parsing arrays to get the main array back

            bool commaParsed = false;

            foreach (char type in types)
            {
                // skip leading comma
                if (type == ',' && !commaParsed)
                {
                    commaParsed = true;
                    continue;
                }

                switch (type)
                {
                    case '\0':
                        break;

                    case 'i':
                        arguments.Add(GetInt(msg, ref index));
                        break;

                    case 'f':
                        arguments.Add(GetFloat(msg, ref index));
                        break;

                    case 's':
                        Debug.WriteLine($"Before GetString() - Expected Start Index: {index}");
                        arguments.Add(GetString(msg, ref index));
                        break;

                    case 'b':
                        arguments.Add(GetBlob(msg, ref index));
                        break;

                    case 'h':
                        arguments.Add(GetLong(msg, ref index));
                        break;

                    case 't':
                        arguments.Add(new Timetag(GetULong(msg, ref index)));
                        break;

                    case 'd':
                        arguments.Add(GetDouble(msg, ref index));
                        break;

                    case 'S':
                        arguments.Add(new Symbol(GetString(msg, ref index)));
                        break;

                    case 'c':
                        arguments.Add(GetChar(msg, ref index));
                        break;

                    case 'r':
                        arguments.Add(GetRGBA(msg, ref index));
                        break;

                    case 'm':
                        arguments.Add(GetMidi(msg, ref index));
                        break;

                    case 'T':
                        arguments.Add(true);
                        break;

                    case 'F':
                        arguments.Add(false);
                        break;

                    case 'N':
                        arguments.Add(null);
                        break;

                    case 'I':
                        arguments.Add(double.PositiveInfinity);
                        break;

                    case '[':
                        if (arguments != mainArray)
                            throw new InvalidOperationException("Nested arrays are not supported.");
                        arguments = [];
                        break;

                    case ']':
                        mainArray.Add(arguments); // add the array to the main array
                        arguments = mainArray; // make arguments point back to the main array
                        break;

                    default:
                        throw new InvalidOperationException($"OSC type tag '{type}' is unknown.");
                }

                AlignIndex(ref index);
            }

            return new OscMessage(address, arguments.ToArray());
        }

        private static OscBundle parseBundle(byte[] bundle)
        {
            if (bundle.Length < 16)
                throw new ArgumentException("Bundle is too small to contain valid OSC data.");

            int index = 0;

            var bundleTag = Encoding.ASCII.GetString(bundle, 0, 8);
            Debug.WriteLine($"Bundle tag: {bundleTag}");
            index += 8;

            UInt64 timetag = GetULong(bundle, ref index);
            Debug.WriteLine($"Timetag extracted: {timetag}, New index: {index}");

            if (bundleTag != "#bundle\0")
                throw new Exception("Not a valid OSC bundle");

            List<OscMessage> messages = new List<OscMessage>();

            while (index < bundle.Length)
            {
                if (index + 4 > bundle.Length)
                {
                    Debug.WriteLine("Warning: Not enough bytes left to read the size.");
                    break;
                }

                int size = GetInt(bundle, ref index);
                Debug.WriteLine($"Extracted Message Size: {size} at index {index}");

                if (size < 0 || index + size > bundle.Length)
                {
                    Debug.WriteLine("Warning: Message size exceeds available data.");
                    break;
                }

                byte[] messageBytes = bundle.Skip(index).Take(size).ToArray();
                Debug.WriteLine($"Extracted Message Bytes: {BitConverter.ToString(messageBytes)}");

                var message = ParseMessage(messageBytes);
                messages.Add(message);

                index += size;
                while (index % 4 != 0)
                    index++;
            }
            Debug.WriteLine($"Total messages extracted: {messages.Count}");
            return new OscBundle(timetag, messages.ToArray());
        }


        #endregion

        #region Get arguments from byte array

        private static string GetAddress(byte[] msg, ref int index)
        {
            int start = index;
            int end = Array.IndexOf(msg, (byte)',', start);
            if (end == -1)
                throw new InvalidOperationException("No comma found in address.");

            index = end + 1;
            return Encoding.ASCII.GetString(msg, start, end - start).TrimEnd('\0');
        }

        private static char[] GetTypes(byte[] msg, ref int index)
        {
            int start = index;
            int end = Array.IndexOf(msg, (byte)0, start);
            if (end == -1)
                throw new InvalidOperationException("No null terminator after type string.");

            index = end + 1;
            return Encoding.ASCII.GetChars(msg, start, end - start);
        }

        private static int GetInt(byte[] msg, ref int index)
        {
            if (index + 4 > msg.Length)
                throw new ArgumentException("Not enough bytes left to read an integer.");

            byte[] intBytes = msg.AsSpan(index, 4).ToArray();
            Debug.WriteLine($"Raw size bytes: {BitConverter.ToString(intBytes)} at index {index}");

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }
            int val = BitConverter.ToInt32(intBytes, 0);
            index += 4;
            return val;
        }



        private static float GetFloat(byte[] msg, ref int index)
        {
            byte[] floatBytes = msg.AsSpan(index, 4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(floatBytes);
            }
            float val = BitConverter.ToSingle(floatBytes, 0);
            index += 4;
            return val;
        }

        private static string GetString(byte[] msg, ref int index)
        {
            int start = index;
            int end = Array.IndexOf(msg, (byte)0, start);

            if (end == -1)
                throw new InvalidOperationException($"No null terminator after string at index {index}.");

            string extracted = Encoding.ASCII.GetString(msg, start, end - start).TrimEnd('\0');
            Debug.WriteLine($"Extracted String: \"{extracted}\" at index {index} (End: {end})");

            index = end + 1;
            return extracted;
        }


        private static byte[] GetBlob(byte[] msg, ref int index)
        {
            int size = GetInt(msg, ref index);
            byte[] blob = msg.AsSpan(index, size).ToArray();
            index += size;
            return blob;
        }

        private static UInt64 GetULong(byte[] msg, ref int index)
        {
            UInt64 val = BitConverter.ToUInt64(msg, index);
            index += 8;
            return val;
        }

        private static Int64 GetLong(byte[] msg, ref int index)
        {
            Int64 val = BitConverter.ToInt64(msg, index);
            index += 8;
            return val;
        }

        private static double GetDouble(byte[] msg, ref int index)
        {
            double val = BitConverter.ToDouble(msg, index);
            index += 8;
            return val;
        }

        private static char GetChar(byte[] msg, ref int index)
        {
            char val = (char)msg[index + 3];
            index += 4;
            return val;
        }

        private static RGBA GetRGBA(byte[] msg, ref int index)
        {
            var rgba = new RGBA(msg[index], msg[index + 1], msg[index + 2], msg[index + 3]);
            index += 4;
            return rgba;
        }

        private static Midi GetMidi(byte[] msg, ref int index)
        {
            var midi = new Midi(msg[index], msg[index + 1], msg[index + 2], msg[index + 3]);
            index += 4;
            return midi;
        }

        private static void AlignIndex(ref int index)
        {
            while (index % 4 != 0)
                index++;
        }

        #endregion

        #region Create byte arrays for arguments

        protected static byte[] SetInt(int value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }
            return intBytes;
        }

        protected static byte[] SetFloat(float value)
        {
            byte[] floatBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(floatBytes);  // Convert to Big Endian
            }
            return floatBytes;
        }

        protected static byte[] SetString(string value)
        {
            int len = value.Length + 1; // +1 for null terminator
            while (len % 4 != 0) len++; // Ensure 4-byte alignment

            byte[] msg = new byte[len];
            Encoding.ASCII.GetBytes(value).CopyTo(msg, 0);
            return msg;
        }

        protected static byte[] GetBlob(byte[] value)
        {
            int len = value.Length + 4;
            len += (4 - len % 4);

            byte[] msg = new byte[len];
            SetInt(value.Length).CopyTo(msg, 0);
            value.CopyTo(msg, 4);
            return msg;
        }

        protected static byte[] SetLong(Int64 value) => BitConverter.GetBytes(value);

        protected static byte[] SetULong(UInt64 value) => BitConverter.GetBytes(value);

        protected static byte[] SetDouble(double value) => BitConverter.GetBytes(value);

        protected static byte[] SetChar(char value)
        {
            byte[] output = new byte[4];
            output[3] = (byte)value;
            return output;
        }

        protected static byte[] SetRGBA(RGBA value) => new byte[] { value.R, value.G, value.B, value.A };

        protected static byte[] SetMidi(Midi value) => new byte[] { value.Port, value.Status, value.Data1, value.Data2 };

        #endregion
    }
}
