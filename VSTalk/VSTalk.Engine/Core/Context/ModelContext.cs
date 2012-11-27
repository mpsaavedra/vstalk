using System.Collections.ObjectModel;
using System.Linq;
using VSTalk.Engine.Model;
using VSTalk.Model;
using VsTalk.Model;

namespace VSTalk.Engine.Core.Context
{
    public class ModelContext : IModelContext
    {
        private Account _account;

        public Account Account
        {
            get { return _account; }
            set { _account = value; }
        }

        public ObservableCollection<Client> Clients
        {
            get { return _account.XmppClients; }
        }

        public ModelContext()
        {
        }

        public Client GetClientById(VsJid id)
        {
            return _account.XmppClients.FirstOrDefault(client => client.Id == id);
        }

        public Interlocutor GetContactById(VsJid id)
        {
            return _account.XmppClients.SelectMany(client => client.Contacts)
                .FirstOrDefault(client => client.Id == id);
        }

        public Client GetClientByContact(Interlocutor interlocutor)
        {
            return _account.XmppClients.FirstOrDefault(client => client.Contacts.Contains(interlocutor));
        }
    }
}