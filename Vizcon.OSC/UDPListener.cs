using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Vizcon.OSC
{
    public delegate void HandleOscPacket(OscPacket packet);
    public delegate void HandleBytePacket(byte[] packet);

    public class UDPListener : IDisposable
    {
        public int Port { get; private set; }

        private readonly object callbackLock;

        readonly UdpClient? receivingUdpClient;
        IPEndPoint? RemoteIpEndPoint;

        readonly HandleBytePacket? BytePacketCallback = null;
        readonly HandleOscPacket? OscPacketCallback = null;

        readonly Queue<byte[]> queue;
        readonly ManualResetEvent ClosingEvent;

        public UDPListener(int port)
        {
            Port = port;
            queue = new Queue<byte[]>();
            ClosingEvent = new ManualResetEvent(false);
            callbackLock = new object();

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    receivingUdpClient = new UdpClient(port);
                    break;
                }
                catch (Exception)
                {
                    if (i >= 9)
                        throw;

                    Thread.Sleep(5);
                }
            }
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            AsyncCallback callBack = new(ReceiveCallback);
            receivingUdpClient?.BeginReceive(callBack, null);
        }

        public UDPListener(int port, HandleOscPacket callback) : this(port)
        {
            OscPacketCallback = callback;
        }

        public UDPListener(int port, HandleBytePacket callback) : this(port)
        {
            BytePacketCallback = callback;
        }

        void ReceiveCallback(IAsyncResult result)
        {
            Monitor.Enter(callbackLock);
            byte[]? bytes = null;

            try
            {
                bytes = receivingUdpClient?.EndReceive(result, ref RemoteIpEndPoint);
            }
            catch (ObjectDisposedException e)
            {
                throw new Exception("Error receiving UDP packet", e);
            }

            // Process bytes
            if (bytes != null && bytes.Length > 0)
            {
                if (BytePacketCallback != null)
                {
                    BytePacketCallback(bytes);
                }
                else if (OscPacketCallback != null)
                {
                    OscPacket? packet = null;

                    try
                    {
                        packet = OscPacket.GetPacket(bytes);

                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error parsing OSC packet", e);
                    }

                    OscPacketCallback(packet);
                }
                else
                {
                    lock (queue)
                    {
                        queue.Enqueue(bytes);
                    }
                }
            }

            if (closing)
                ClosingEvent.Set();
            else
            {
                // Setup next async event
                AsyncCallback callBack = new(ReceiveCallback);
                receivingUdpClient?.BeginReceive(callBack, null);
            }
            Monitor.Exit(callbackLock);
        }

        bool closing = false;
        public void Close()
        {
            lock (callbackLock)
            {
                ClosingEvent.Reset();
                closing = true;
                receivingUdpClient?.Close();
            }
            ClosingEvent.WaitOne();

        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public OscPacket? Receive()
        {
            if (closing) throw new Exception("UDPListener has been closed.");

            lock (queue)
            {
                if (queue.Count > 0)
                {
                    byte[] bytes = queue.Dequeue();
                    var packet = OscPacket.GetPacket(bytes);
                    return packet;
                }
                else
                    return null;
            }
        }

        public byte[]? ReceiveBytes()
        {
            if (closing) throw new Exception("UDPListener has been closed.");

            lock (queue)
            {
                if (queue.Count > 0)
                {
                    byte[] bytes = queue.Dequeue();
                    return bytes;
                }
                else
                    return null;
            }
        }

    }
}
