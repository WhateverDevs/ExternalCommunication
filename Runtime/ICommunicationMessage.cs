public interface ICommunicationMessage
{
    void FromByteArray(byte[] data);

    byte[] ToByteArray();
}
