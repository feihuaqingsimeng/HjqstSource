namespace Logic.Protocol
{
    public interface IProtocol : ProtoBuf.IExtensible
    {
        byte[] ToBytes();
        void FromBytes(Logic.Protocol.MBinaryReader mbr);
        EProtocolId GetProtocolId();
    }
}
