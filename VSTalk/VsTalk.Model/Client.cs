using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using VSTalk.Engine.Model;
using VSTalk.Tools;

namespace VSTalk.Model
{
    [DataContract]
    public class Client : INotifyPropertyChanged
    {
        private ObservableCollection<Interlocutor> contacts;

        private VsJid id;

        private string login;

        private string password;

        private ClientState state = ClientState.Disconnected;

        private bool enabled;

        private bool useSsl;

        private string server;

        #region PROPERTIES
        [DataMember]
        public string Server
        {
            get { return server; }
            set
            {
                server = value;
                PropertyChanged.Notify(() => Server);
            }
        }

        [DataMember]
        public bool UseSsl
        {
            get { return useSsl; }
            set
            {
                useSsl = value;
                PropertyChanged.Notify(() => UseSsl);
            }
        }

        [DataMember]
        public ObservableCollection<Interlocutor> Contacts
        {
            get { return contacts; }
            set
            {
                contacts = value;
                PropertyChanged.Notify(() => Contacts);
            }
        }

        [DataMember]
        public VsJid Id
        {
            get { return id; }
            set
            {
                id = value;
                PropertyChanged.Notify(() => Id);
            }
        }
        [DataMember]
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                PropertyChanged.Notify(() => Login);
            }
        }

        [DataMember]
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                PropertyChanged.Notify(() => Password);
            }
        }

        [DataMember]
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                PropertyChanged.Notify(() => Enabled);
            }
        }

        public ClientState State
        {
            get { return state; }
            set
            {
                state = value;
                if (state != ClientState.Connected)
                {
                    Contacts.Each(contact =>
                    {
                        contact.State = ContactState.Offline;
                    });
                }
                PropertyChanged.Notify(() => State);
            }
        }

        #endregion

        public Client()
        {
            Contacts = new ObservableCollection<Interlocutor>();
            State = ClientState.Disconnected;
        }

        [OnDeserializing]
        internal void OnDeserializing(StreamingContext context)
        {
            if (Contacts == null)
            {
                Contacts = new ObservableCollection<Interlocutor>();
            }

            State = ClientState.Disconnected;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}