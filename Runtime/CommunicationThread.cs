using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


    public class CommunicationThread : ThreadedJob
    {
        private Queue<int> deltaTimesThread = new Queue<int>();
        DateTime lastTime = DateTime.Now;

        public event Action<byte[]> MessageReceived;

        public event Action ExceptionRaised;

        public string lastMessageSent;

        public bool Connected { get; private set; }

        public volatile bool EndThread = false;

        private IPEndPoint endPoint;
        private Socket socket;

        private byte[] buffer;

        public CommunicationThread(string ip, int port, SocketType socketType, ProtocolType protocolType, int bufferSize)
        {
            buffer = new byte[bufferSize];
            socket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.Connect(endPoint);
            Connected = socket.Connected;
        }

        public int Send(byte[] data)
        {
            return socket.Send(data, 0, data.Length, SocketFlags.None);
        }

        private void Receive()
        {
            try
            {
                int received = socket.Receive(buffer);
                
                MessageReceived?.Invoke(buffer);
                
                /*SimulatorMessage a = new SimulatorMessage();
                a.FromByteArray(buffer);
                Debug.Log("ID " + a._info.DATA.ID + "TIMESTAMP " + a._info.DATA.BLOCKS[0].TIMESTAMP);*/
            }
            catch (SocketException e)
            {
                Debug.LogError("There was an exception on Receive: " + e.ErrorCode + " mensaje " + e.Message);

                MessageReceived?.Invoke(new byte[]
                                        {
                                        }); // Send this empty message to trigger a reinitialization attempt

                ExceptionRaised?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("There was an exception on Receive: " + e.Message);
            }
        }

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

        protected override void OnFinished()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Sending shutdown to port ").Append(endPoint.Port);
            Debug.Log(sb.ToString());
            socket.Shutdown(SocketShutdown.Send);
            socket.Close();
            Connected = false;

            float threadTimes = 0;

            for (int i = deltaTimesThread.Count / 2; i < deltaTimesThread.Count; ++i)
            {
                threadTimes += deltaTimesThread.ElementAt(i);
            }

            float threadFreq = threadTimes / (deltaTimesThread.Count - deltaTimesThread.Count / 2);
            Debug.LogWarning("Average timeStamp thread: " + threadFreq);
        }
    }
