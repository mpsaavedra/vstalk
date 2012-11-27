using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using VSTalk.Engine.Core;
using VSTalk.Engine.Utils;
using VSTalk.Engine.Windows;
using VSTalk.Model;
using VSTalk.Tools;

namespace VSTalk.Engine.ViewModel.ContactList
{
    public class ContactListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly VSTalkCore _core;

        private ObservableCollection<ClientNodeViewModel> _clients = new ObservableCollection<ClientNodeViewModel>();

        public ObservableCollection<ClientNodeViewModel> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                PropertyChanged.Notify(() => Clients);
            }
        }

        public RelayCommand<Interlocutor> ShowInterlocutorChat { get; private set; }

        public RelayCommand<Interlocutor> ShowContactLog { get; private set; }

        public RelayCommand<Interlocutor> SendRequestAuthorization { get; private set; }
        
        public RelayCommand<Interlocutor> RemoveContact { get; private set; }

        public RelayCommand<Client> ShowAddContactWindow { get; private set; }

        public RelayCommand<Client> ConnectClient { get; private set; }

        public ContactListViewModel(VSTalkCore core)
        {
            _core = core;

            SetCommands();

            FillContactList();
        }

        private void FillContactList()
        {
            var xmppClients = _core.ModelContext.Clients;
            xmppClients.HandleCollectionChanges(initial: AddClientPresentation,
                                                added: AddClientPresentation,
                                                removed: RemoveClientPresentation);
        }

        private void AddClientPresentation(Client client)
        {
            Clients.Add(new ClientNodeViewModel(client));
        }

        private void RemoveClientPresentation(Client client)
        {
            Clients.Remove(Clients.First(model => model.Client == client));
        }

        private void SetCommands()
        {
            ShowInterlocutorChat = new RelayCommand<Interlocutor>(ShowInterlocutorChatExecuted);
            ShowContactLog = new RelayCommand<Interlocutor>(ShowContactLogExecuted);
            SendRequestAuthorization = new RelayCommand<Interlocutor>(SendRequestAuthorizationExecuted, SendRequestAuthorizationCanExecute);
            RemoveContact = new RelayCommand<Interlocutor>(RemoveContactExecuted, RemoveContactCanExecute);
            ShowAddContactWindow = new RelayCommand<Client>(ShowAddContactWindowExecuted);
            ConnectClient = new RelayCommand<Client>(ConnectClientExecuted);
        }

        private void ConnectClientExecuted(Client client)
        {
            _core.Connector.Connect(client);
        }

        private void ShowAddContactWindowExecuted(Client client)
        {
            var contact = new AddContactWindow(_core, client);
            contact.Show();
        }

        protected void RemoveContactExecuted(Interlocutor interlocutor)
        {
            _core.GetClientService(interlocutor).RemoveContact(interlocutor);
        }

        protected bool RemoveContactCanExecute(Interlocutor interlocutor)
        {
            return interlocutor != null && _core.ModelContext.GetClientByContact(interlocutor).State == ClientState.Connected;
        }

        protected void SendRequestAuthorizationExecuted(Interlocutor interlocutor)
        {
            _core.GetClientService(interlocutor).SendRequestAuthorization(interlocutor);
        }

        protected bool SendRequestAuthorizationCanExecute(Interlocutor interlocutor)
        {
            return interlocutor != null && !interlocutor.Subscribed && _core.ModelContext.GetClientByContact(interlocutor).State == ClientState.Connected;
        }

        protected void ShowContactLogExecuted(Interlocutor interlocutor)
        {
            var historyWindow = new HistoryWindow(interlocutor, _core.ModelContext);
            historyWindow.Show();
        }

        protected void ShowInterlocutorChatExecuted(Interlocutor interlocutor)
        {
            _core.ShowInterlocutorChat(interlocutor);
        }
    }
}