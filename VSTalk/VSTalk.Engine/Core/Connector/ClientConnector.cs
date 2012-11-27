using System.Linq;
using VSTalk.Engine.Core.Context;
using VSTalk.Model;
using VSTalk.Tools;

namespace VSTalk.Engine.Core.Connector
{
    public class ClientConnector : IConnector
    {
        private readonly IConnectionContainer _container;

        public ClientConnector(IConnectionContainer container)
        {
            _container = container;
        }

        public void Connect(Client client)
        {
            Connect(TakeConnection(client));   
        }
         
        public void Disconnect(Client client)
        {
            Disconnect(TakeConnection(client));
        }

        private void Connect(ClientConnection connection)
        {
            if (connection.Client.State == ClientState.Disconnected)
            {
                connection.Connect();
            }
        }

        private void Disconnect(ClientConnection connection)
        {
            if (connection.Client.State != ClientState.Disconnected)
            {
                connection.Disconnect();
            }
        }
        
        public void ConnectClients()
        {
            _container.AvailableConnections.Where(connection => connection.Client.Enabled).Each(Connect);
        }

        public void DisconnectClients()
        {
            _container.AvailableConnections.Each(Disconnect);
        }

        private ClientConnection TakeConnection(Client client)
        {
            return _container.AvailableConnections.First(serv => serv.Client == client);
        }
    }
}