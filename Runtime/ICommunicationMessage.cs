namespace WhateverDevs.ExternalCommunication.Runtime
{
    /// <summary>
    ///     Interface to create your own communication message
    /// </summary>
    public interface ICommunicationMessage
    {
        /// <summary>
        ///     Translate from byte array
        /// </summary>
        /// <param name="data">Byte arrary</param>
        void FromByteArray(byte[] data);

        /// <summary>
        ///     Translate to byte array
        /// </summary>
        /// <returns>Byte array info</returns>
        byte[] ToByteArray();
    }
}