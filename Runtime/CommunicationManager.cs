using System;
using System.Diagnostics;
using System.Net.Sockets;
using UnityEditor.VersionControl;
using WhateverDevs.Core.Runtime.Common;
using Debug = UnityEngine.Debug;

public abstract class CommunicationManager : Loggable<CommunicationManager>
{
    public static long timestamp = 0;

    protected ExternalCommunicationConfigurationData configurationData;
    protected CommunicationThread communicationThread;
    private volatile bool dataReceived = false;
    
    protected bool threadReady = false;
    
    public abstract void SendFirstMessage();

    public abstract void SendLastMessage();
    
    public bool CheckProcess()
    {
        Process[] processes = Process.GetProcessesByName(configurationData.processName);
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

    public void SetupThreads()
    {
        try
        {
            if (!threadReady)
            {
                communicationThread = new CommunicationThread(configurationData.ipAddress,configurationData.port,configurationData.socketType,configurationData.protocolType,configurationData.bufferSize);
                threadReady = communicationThread.Connected;
                communicationThread.Start();
            }
        }
        catch (Exception e)
        {
            GetLogger().Error("Socket error:" + e);
            CloseThread();
        }
    }
    
    public void CloseThread()
    {
        threadReady = false;
        communicationThread.EndThread = true;
    }
}
