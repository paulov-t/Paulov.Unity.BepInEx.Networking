namespace Paulov.UnityBepInExNetworking
{
    public interface IPaulovNetworkingPacket
    {
        public string Method { get; }
        public int MethodHash { get; set; }
        byte[] Serialize();
        IPaulovNetworkingPacket Deserialize(byte[] bytes);
        void Process();

    }

}
