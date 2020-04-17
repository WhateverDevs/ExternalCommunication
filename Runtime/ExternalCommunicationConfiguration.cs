using System;
using System.Net.Sockets;
using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;

namespace WhateverDevs.ExternalCommunication.Runtime
{
    [CreateAssetMenu(menuName = "WhateverDevs/ExternalToolCommunication/Configuration",
        fileName = "CommunicationConfiguration")]
    public class ExternalCommunicationConfiguration : ConfigurationScriptableHolderUsingFirstValidPersister<
        ExternalCommunicationConfigurationData>
    {
    }

    /// <summary>
    ///     ExternalCommunicationConfigurationData configuration data.
    /// </summary>
    [Serializable]
    public class ExternalCommunicationConfigurationData : ConfigurationData
    {
        public string ProcessName;
        public string IpAddress;
        public int Port;
        public SocketType SocketType;
        public ProtocolType ProtocolType;
        public int BufferSize = 1024;
    }
}