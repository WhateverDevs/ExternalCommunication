using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WhateverDevs.ExternalCommunication.Runtime
{
    /// <summary>
    ///     CommunicationThread class
    /// </summary>
    public class CommunicationThread : ThreadedJob
    {
        private readonly Queue<int> deltaTimesThread = new Queue<int>();
        private DateTime lastTime = DateTime.Now;

        public event Action<byte[]> MessageReceived;

        public event Action ExceptionRaised;

        public string LastMessageSent;

        public bool Connected { get; private set; }

        public volatile bool EndThread = false;

        private readonly IPEndPoint endPoint;
        private readonly Socket socket;

        private readonly byte[] buffer;

        /// <summary>
        ///     Setting a connection via socket
        /// </summary>
        /// <param name="ip">ip address</param>
        /// <param name="port">port number</param>
        /// <param name="socketType">Socket Type</param>
        /// <param name="protocolType">Protocol Type</param>
        /// <param name="bufferSize">Buffer Size</param>
        public CommunicationThread(string ip,
                                   int port,
                                   SocketType socketType,
                                   ProtocolType protocolType,
                                   int bufferSize)
        {
            buffer = new byte[bufferSize];
            socket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.Connect(endPoint);
            Connected = socket.Connected;
        }

        /// <summary>
        ///     Send the bytes using the socket
        /// </summary>
        /// <param name="data">Data in bytearray</param>
        /// <returns>Number of bytes sent</returns>
        public int Send(byte[] data) => socket.Send(data, 0, data.Length, SocketFlags.None);

        /// <summary>
        ///     Receive
        /// </summary>
        private void Receive()
        {
            try
            {
                int received = socket.Receive(buffer);

                MessageReceived?.Invoke(buffer);
            }
            catch (SocketException e)
            {
                GetLogger().Error("There was an exception on Receive: " + e.ErrorCode + " mensaje " + e.Message);

                MessageReceived?.Invoke(new byte[]
                                        {
                                        }); // Send this empty message to trigger a reinitialization attempt

                ExceptionRaised?.Invoke();
            }
            catch (Exception e)
            {
                GetLogger().Error("There was an exception on Receive: " + e.Message);
            }
        }

        /// <summary>
        ///     Thread function
        /// </summary>
        protected override void ThreadFunction()
        {
            while (!EndThread)
            {
                Receive();
                deltaTimesThread.Enqueue(DateTime.Now.Subtract(lastTime).Milliseconds);
                lastTime = DateTime.Now;
            }

            OnFinished();
        }

        /// <summary>
        ///     OnFinished the thread
        /// </summary>
        protected override void OnFinished()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Sending shutdown to port ").Append(endPoint.Port);
            GetLogger().Debug(sb.ToString());
            socket.Shutdown(SocketShutdown.Send);
            socket.Close();
            Connected = false;

            float threadTimes = 0;

            for (int i = deltaTimesThread.Count / 2; i < deltaTimesThread.Count; ++i)
                threadTimes += deltaTimesThread.ElementAt(i);

            float threadFreq = threadTimes / (deltaTimesThread.Count - deltaTimesThread.Count / 2);
            GetLogger().Warn("Average timeStamp thread: " + threadFreq);
        }
    }
}