using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VSTalk.Model;

namespace VsTalk.Model
{
    [DataContract]
    public class Account
    {
        private ObservableCollection<Client> _xmppClients;

        [DataMember]
        public ObservableCollection<Client> XmppClients
        {
            get { return _xmppClients; }
            set { _xmppClients = value; }
        }

        public Account()
        {
            _xmppClients = new ObservableCollection<Client>();
        }

    }
}
