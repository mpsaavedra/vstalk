using VSTalk.Model;

namespace VSTalk.Engine.Core.Connector
{
    public interface IConnector
    {
        void Connect(Client client);
        void Disconnect(Client client);
        void ConnectClients();
        void DisconnectClients();
    }
}