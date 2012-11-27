using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;
using VSTalk.Engine.Utils;
using VSTalk.Tools;
using VSTalk.Model;

namespace VSTalk.Engine.Windows
{
    /// <summary>
    /// Interaction logic for AccountsWindow.xaml
    /// </summary>
    public partial class AccountsWindow : MetroWindow, INotifyPropertyChanged
    {
        public RelayCommand AddNewClient { get; set; }

        public RelayCommand RemoveClient { get; set; }

        public ObservableCollection<Client> Clients
        {
            get { return _core.ModelContext.Account.XmppClients; }
        }

        private Client selectedClient;

        public Client SelectedClient
        {
            get
            {
                return selectedClient;
            }
            set
            {
                selectedClient = value;
                PropertyChanged.Notify(() => SelectedClient);
            }
        }

        private readonly Core.VSTalkCore _core;

        public AccountsWindow(Core.VSTalkCore core)
        {
            InitializeComponent();
            DataContext = this;
            _core = core;

            SetCommands();
        }

        private void SetCommands()
        {
            AddNewClient = new RelayCommand(AddNewClientExecuted);
            RemoveClient = new RelayCommand(RemoveClientExecuted,
                                            () => SelectedClient != null);
        }

        protected void RemoveClientExecuted()
        {
            _core.RemoveClient(SelectedClient);
        }

        protected void AddNewClientExecuted()
        {
            var clientWindow = new ClientWindow();
            var result = clientWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                _core.AddClient(clientWindow.Client);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    

}
