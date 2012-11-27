using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using VSTalk.Engine.Core;
using VSTalk.Engine.Utils;
using VSTalk.Engine.ViewModel.ContactList;
using VSTalk.Engine.Windows;
using VSTalk.Tools;
using VSTalk.Model;

namespace VSTalk.Engine.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly VSTalkCore _core;

        public RelayCommand ChangeConnectionState { get; private set; }

        public RelayCommand ShowAccountsWindow { get; private set; }

        public RelayCommand ShowAddContactWindow { get; private set; }

        public RelayCommand AddNewAccount { get; private set; }

        public RelayCommand ChangeAppSettings { get; private set; }

        public ContactListViewModel ContactList { get; private set; }

        public NotificationViewModel NotificationView { get; private set; }
        
        private ObservableCollection<Client> Clients
        {
            get { return _core.ModelContext.Clients; }
        }

        public ClientState CommonState
        {
            get
            {
                if (Clients.Any(client => client.State == ClientState.Connecting))
                {
                    return ClientState.Connecting;
                }

                if (Clients.Where(client => client.Enabled).All(client => client.State == ClientState.Connected))
                {
                    return ClientState.Connected;
                }

                return ClientState.Disconnected;
            }
        }

        public bool HasConnections
        {
            get { return Clients.Count() > 0; }
        }
        
        public MainViewModel(VSTalkCore core)
        {
            _core = core;
            
            ContactList = new ContactListViewModel(core);
            NotificationView = new NotificationViewModel(core.NotificationQueue);

            SetHandlers();
            SetCommands();
        }

        private void SetCommands()
        {
            ChangeConnectionState = new RelayCommand(ChangeConnectionStateExecuted);
            ShowAccountsWindow = new RelayCommand(ShowAccountsWindowExecuted);
            ShowAddContactWindow = new RelayCommand(ShowAddContactWindowExecuted);
            ChangeAppSettings = new RelayCommand(ChangeAppSettingsExecuted);
            AddNewAccount = new RelayCommand(AddNewAccountExecuted);
        }

        private void AddNewAccountExecuted()
        {
            var clientWindow = new ClientWindow();
            var result = clientWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                _core.AddClient(clientWindow.Client);
            }
        }

        private void SetHandlers()
        {
            PropertyChanged.Notify(() => HasConnections);
            PropertyChanged.Notify(() => CommonState);

            Clients.CollectionChanged += delegate { PropertyChanged.Notify(() => HasConnections); };
            Clients.HandleInnerChanges(innerSelector: client => client.State,
                                       handler: state => PropertyChanged.Notify(() => CommonState));
        }

        protected void ChangeAppSettingsExecuted()
        {
            Properties.Settings.Default.Save();
        }

        protected void ShowAddContactWindowExecuted()
        {
            var addContactWindow = new AddContactWindow(_core);
            addContactWindow.ShowDialog();
        }

        protected void ShowAccountsWindowExecuted()
        {
            var accWindow = new AccountsWindow(_core);
            accWindow.ShowDialog();
        }

        public void ChangeConnectionStateExecuted()
        {
            switch (CommonState)
            {
                case ClientState.Disconnected:
                    _core.Connector.ConnectClients();
                    break;
                case ClientState.Connected:
                    _core.Connector.DisconnectClients();
                    break;
            }
        }
    }
}