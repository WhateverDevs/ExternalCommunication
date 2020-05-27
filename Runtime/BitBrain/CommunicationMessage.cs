using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using log4net;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Serialization;
using WhateverDevs.ExternalCommunication.Runtime;
using Debug = UnityEngine.Debug;

namespace ExternalCommunicationDev
{

    [Serializable]
    public class BLOCK
    {
        public long Timestamp;
        public int[] Values;
    }

    [Serializable]
    public class DATA
    {
        public BLOCK[] Blocks;
        public string Id;
    }

    [Serializable]
    public class RootObject
    {
        public DATA Data;

        public RootObject()
        {
            Data = new DATA();
            Data.Blocks = new BLOCK[1];
            BLOCK temp = new BLOCK();
            temp.Timestamp = Stopwatch.GetTimestamp();
            temp.Values = new int[1];
            temp.Values[0] = 1;
            Data.Blocks[0] = temp;
        }
    }

    [Serializable]
    public class CommunicationMessage : Loggable<CommunicationMessage>, ICommunicationMessage
    {
        public RootObject Info;
        private readonly JsonSerializer serializer;

        public CommunicationMessage()
        {
            serializer = new JsonSerializer();
            Info = new RootObject();
        }

        public CommunicationMessage(byte[] data) => FromByteArray(data);

        private int amount;
        private readonly StringBuilder bufferString = new StringBuilder();

        public void FromByteArray(byte[] data)
        {
            List<string> allJsons = new List<string>();
            string allDataString = Encoding.UTF8.GetString(data);

            int index = 0;

            while (index < allDataString.Length - 1)
                for (int i = index; i < allDataString.Length; ++i)
                {
                    bufferString.Append(allDataString[i]);

                    if (allDataString[i] == '{')
                        ++amount;
                    else if (allDataString[i] == '}')
                    {
                        --amount;

                        if (amount == 0)
                        {
                            allJsons.Add(bufferString.ToString());
                            bufferString.Clear();
                            index = i;
                            ++index;
                            break;
                        }
                    }

                    index = i;
                }

            for (int i = 0; i < allJsons.Count; ++i)
            {
                RootObject a = serializer.From<RootObject>(allJsons[i]); //todo change
                Info = a;
                BitBrainComunication.Timestamp = a.Data.Blocks[0].Timestamp;
            }
        }

        public byte[] ToByteArray()
        {
            Info.Data.Blocks[0].Timestamp = Stopwatch.GetTimestamp();
            string resultString = JsonUtility.ToJson(Info).ToUpper();//todo modify 
            GetLogger().Debug("TIMESTAMP" + Info.Data.Blocks[0].Timestamp);
            
            byte[] bArray = Encoding.UTF8.GetBytes(resultString);
            //Encoding.UTF8.GetBytes("{\"DATA\":{\"BLOCKS\":[{\"TIMESTAMP\":" + Info.Data.Blocks[0].Timestamp + ",\"VALUES\":[" + Info.Data.Blocks[0].Values[0] + "]}],\"ID\":\""+ Info.Data.Id + "\"}}"); //(resultString);

            return bArray;
        }
    }
}