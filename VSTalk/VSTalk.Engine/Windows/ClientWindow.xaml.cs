using System.Windows;
using MahApps.Metro.Controls;
using VSTalk.Engine.Utils;
using VSTalk.Model;

namespace VSTalk.Engine.Windows
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : MetroWindow
    {
        public Client Client { get; private set; }

        public RelayCommand AddClient { get; set; }
        
        public RelayCommand CloseWindow { get; set; }

        public ClientWindow()
        {
            InitializeComponent();
            DataContext = this;
            Client = new Client();
            AddClient = new RelayCommand(
                delegate
                {
                    Client.Password = PasswordBox.Password;
                    Client.Enabled = true;
                    DialogResult = true;
                    this.Close();
                },
                delegate
                {
                    return !string.IsNullOrEmpty(Client.Login) &&
                        !string.IsNullOrEmpty(PasswordBox.Password) &&
                        !string.IsNullOrEmpty(Client.Server);
                });
            CloseWindow = new RelayCommand(delegate { Close();});
        }

    }
}
