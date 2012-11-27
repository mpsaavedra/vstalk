using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MahApps.Metro.Controls;
using VSTalk.Engine.Core;
using VSTalk.Engine.Utils;
using VSTalk.Tools;
using VSTalk.Model;

namespace VSTalk.Engine.Windows
{
    /// <summary>
    /// Interaction logic for AddContactWindow.xaml
    /// </summary>
    public partial class AddContactWindow : MetroWindow, INotifyPropertyChanged
    {
        public string ContactName { get; set; }

        public ObservableCollection<Client> Clients { get; set; }

        public RelayCommand AddContact { get; set; }

        public RelayCommand CloseWindow { get; set; }

        private Client selectedClient;

        public Client SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;
                PropertyChanged.Notify(() => SelectedClient);
            }
        }

        private readonly VSTalkCore _core;

        public AddContactWindow(VSTalkCore core, Client client = null)
        {
            InitializeComponent();
            DataContext = this;
            _core = core;
            Clients = new ObservableCollection<Client>(core.AvailableClients);
            SelectedClient = client ?? Clients.FirstOrDefault();
            SetCommands();
        }

        private void SetCommands()
        {
            AddContact = new RelayCommand(AddContactExecuted, () => !string.IsNullOrEmpty(ContactName));
            CloseWindow = new RelayCommand(Close);
        }

        public void AddContactExecuted()
        {
            _core.GetClientService(SelectedClient).AddContact(ContactName);
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
