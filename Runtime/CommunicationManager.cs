using System;
using System.Diagnostics;
using WhateverDevs.Core.Runtime.Common;

namespace WhateverDevs.ExternalCommunication.Runtime
{
    /// <summary>
    ///     Abstract class for CommunicationManager manager.
    /// </summary>
    public abstract class CommunicationManager : Loggable<CommunicationManager>
    {
        /// <summary>
        ///     ConfigurationData for the connection
        /// </summary>
        protected ExternalCommunicationConfigurationData ConfigurationData;

        /// <summary>
        ///     CommunicationThread to manage the communication
        /// </summary>
        protected CommunicationThread CommunicationThread;

        /// <summary>
        ///     Flags to check the thread and data
        /// </summary>
        protected volatile bool DataReceived = false;

        protected bool ThreadReady;

        /// <summary>
        ///     Abstract function to init the manager
        /// </summary>
        public abstract void Init();
        
        /// <summary>
        ///     Abstract function to recieve messages
        /// </summary>
        public abstract void RecieveMessage(byte[] array);
        
        /// <summary>
        ///     Abstract function to send the first message
        /// </summary>
        public abstract void SendFirstMessage();

        /// <summary>
        ///     Abstract function to send the last message
        /// </summary>
        public abstract void SendLastMessage();

        /// <summary>
        ///     Check the process is running
        /// </summary>
        /// <returns>If the process is found</returns>
        public bool CheckProcess()
        {
            Process[] processes = Process.GetProcessesByName(ConfigurationData.ProcessName);
            Process process;

            if (processes.Length != 0)
            {
                process = processes[0];
                GetLogger().Debug("Process found" + process.MachineName);
                return true;
            }

            GetLogger().Error("Process not found");
            return false;
        }

        /// <summary>
        ///     Setup the communicatioThread
        /// </summary>
        public void SetupThreads()
        {
            try
            {
                if (!ThreadReady)
                {
                    CommunicationThread = new CommunicationThread(ConfigurationData.IpAddress,
                                                                  ConfigurationData.Port,
                                                                  ConfigurationData.SocketType,
                                                                  ConfigurationData.ProtocolType,
                                                                  ConfigurationData.BufferSize);

                    ThreadReady = CommunicationThread.Connected;
                    CommunicationThread.Start();
                }
            }
            catch (Exception e)
            {
                GetLogger().Error("Socket error:" + e);
                CloseThread();
            }
        }

        /// <summary>
        ///     Close the communicationThread
        /// </summary>
        public void CloseThread()
        {
            ThreadReady = false;
            CommunicationThread.EndThread = true;
        }
    }
}