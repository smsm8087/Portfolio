public interface INetworkMessageHandler
{
    string Type { get; }
    void Handle(NetMsg msg);
}