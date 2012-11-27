using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VSTalk.Engine.Core.Connector;
using VSTalk.Engine.Core.Context;
using VSTalk.Engine.Core.Controls;
using VSTalk.Engine.Core.Notifications;
using VSTalk.Model;
using VSTalk.Engine.ViewModel;
using VsTalk.Model;

namespace VSTalk.Engine.Core
{
    public class VSTalkCore : IClientContainer, IConnectionContainer
    {
        private readonly IList<ClientConnection> _registredConnections = new List<ClientConnection>();

        private Account Account
        {
            get { return ModelContext.Account; }
        }

        public NotificationQueue NotificationQueue { get; private set; }
        
        public IEnvironmentManager EnvironmentManager { get; private set; }
        
        public IModelContext ModelContext { get; private set; }
        
        public IControlsRepository ControlsRepository { get; private set; }
        
        public IWindowsManager WindowsManager { get; private set; }

        public IConnector Connector { get; private set; }
        
        public VSTalkCore(IWindowsManager windowsManager, IEnvironmentManager environmentManager, IModelContext modelContext)
        {
            WindowsManager = windowsManager;
            EnvironmentManager = environmentManager;
            ModelContext = modelContext;
            
            NotificationQueue = new NotificationQueue();

            Connector = new ClientConnector(this);
            ControlsRepository = new ControlRepository(this);

            LoadConnections();
        }

        private void LoadConnections()
        {
            foreach (var client in Account.XmppClients)
            {
                InitializeConnection(client);
            }
        }

        private ClientConnection CreateConnection(Client client)
        {
            var connection = new ClientConnection(NotificationQueue, client);
            return connection;
        }

        #region Windows

        public void ShowMainWindow()
        {
            WindowsManager.OpenWindow(ControlsRepository.ContactList);
        }

        public void ShowInterlocutorChat(Interlocutor interlocutor)
        {
            WindowsManager.OpenWindow(ControlsRepository.ChatFrame);
            ((ChatFrameViewModel)ControlsRepository.ChatFrame.DataContext).AppendInterlocutor(interlocutor);
        }

        #endregion

        #region ClientConnection Logic

        private ClientConnection TakeConnection(Client client)
        {
            return _registredConnections.FirstOrDefault(wrap => wrap.Client == client);
        }

        private ClientConnection TakeConnection(Interlocutor interlocutor)
        {
            return _registredConnections.FirstOrDefault(wrap => wrap.HasContact(interlocutor));
        }

        public IClientService GetClientService(Interlocutor interlocutor)
        {
            return TakeConnection(interlocutor);
        }

        public IClientService GetClientService(Client client)
        {
            return TakeConnection(client);
        }

        public void AddClient(Client client)
        {
            Account.XmppClients.Add(client);
            InitializeConnection(client);
        }

        private void InitializeConnection(Client client)
        {
            var clientWrapper = CreateConnection(client);
            _registredConnections.Add(clientWrapper);
            if (client.Enabled)
            {
                Connector.Connect(client);
            }
        }

        public void RemoveClient(Client client)
        {
            DisposeConnection(client);
            Account.XmppClients.Remove(client);
        }

        private void DisposeConnection(Client client)
        {
            if (client.State != ClientState.Disconnected)
            {
                Connector.Disconnect(client);
            }
            var connection = TakeConnection(client);
            _registredConnections.Remove(connection);
        }

        public ObservableCollection<Client> AvailableClients
        {
            get { return Account.XmppClients; }
        }

        public IEnumerable<ClientConnection> AvailableConnections
        {
            get { return _registredConnections; }
        }

        #endregion

        public void Unload()
        {
            Connector.DisconnectClients();
            var contextSaver = new ModelContextSaver();
            contextSaver.Save(ModelContext);
        }
    }
}
