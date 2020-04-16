using System;
using System.Net.Sockets;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;


[CreateAssetMenu(menuName = "WhateverDevs/ExternalToolCommunication/Configuration", fileName = "CommunicationConfiguration")]
public class ExternalCommunicationConfiguration : ConfigurationScriptableHolderUsingFirstValidPersister<ExternalCommunicationConfigurationData>
{
    
}

/// <summary>
/// Testable example of configuration data.
/// </summary>
[Serializable]
public class ExternalCommunicationConfigurationData : ConfigurationData
{
    public string processName;
    public string ipAddress;
    public int port;
    public SocketType socketType;
    public ProtocolType protocolType;
    public int bufferSize = 1024;
}
