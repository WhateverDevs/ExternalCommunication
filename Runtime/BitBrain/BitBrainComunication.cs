using System;
using System.Text;
using UnityEngine;
using WhateverDevs.ExternalCommunication.Runtime;

namespace ExternalCommunicationDev
{
    /// <summary>
    ///     BitBrainSampleManager sample to test features
    /// </summary>
    public class BitBrainComunication : CommunicationManager
    {
        public static long Timestamp;

        public void SetConfigurationData(ExternalCommunicationConfigurationData data) => ConfigurationData = data;

        public override void Init()
        {
            CommunicationThread.MessageReceived += RecieveMessage;
        }

        public override void RecieveMessage(byte[] array)
        {
            CommunicationMessage message = new CommunicationMessage();
            message.FromByteArray(array);
            Debug.Log("ID " + message.Info.Data.Id);
        }

        /*
         * ⦁	La plataforma enviará el evento “calibration1 (0)” cuando comience la primera tarea.
         * ⦁	La plataforma enviará el evento “calibration1 (1)” cuando termine la primera tarea.
         * ⦁	La plataforma enviará el evento “calibration2 (0)” cuando comience la segunda tarea.
         * ⦁	La plataforma enviará el evento “calibration2 (1)” cuando termine la segunda tarea.
         */
        
        public void Calibration(int calibration, int value)
        {
            try
            {
                if (ThreadReady)
                {
                    CommunicationMessage msgSimulatorMessage = new CommunicationMessage();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(calibration == 0 ? "calibration1" : "calibration2");
                    msgSimulatorMessage.Info.Data.Blocks[0].Values[0] = value;
                    msgSimulatorMessage.Info.Data.Id = sb.ToString();
                    CommunicationThread.Send(msgSimulatorMessage.ToByteArray());
                }
            }
            catch (Exception e)
            {
                GetLogger().Error("SendFirstMessage error :" + e);
            }
        }


        public override void SendFirstMessage()
        {
            try
            {
                if (ThreadReady)
                {
                    CommunicationMessage msgSimulatorMessage = new CommunicationMessage();
                    msgSimulatorMessage.Info.Data.Id = "protocol_start";
                    CommunicationThread.Send(msgSimulatorMessage.ToByteArray());
                }
            }
            catch (Exception e)
            {
                GetLogger().Error("SendFirstMessage error :" + e);
            }
        }

        public override void SendLastMessage()
        {
            try
            {
                if (ThreadReady)
                {
                    CommunicationMessage msgSimulatorMessage = new CommunicationMessage();
                    msgSimulatorMessage.Info.Data.Id = "protocol_end";
                    CommunicationThread.Send(msgSimulatorMessage.ToByteArray());
                }
            }
            catch (Exception e)
            {
                GetLogger().Error("Last message error :" + e);
            }
        }
    }
}