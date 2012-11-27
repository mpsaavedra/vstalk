using System.Linq;
using VSTalk.Engine.Model;
using VSTalk.Model;

namespace VSTalk.Engine.Core.RosterObserver
{
    public class RosterContainer : IRosterContainer
    {
        private readonly Client _client;

        public RosterContainer(Client client)
        {
            _client = client;
        }

        public bool Contains(VsJid id)
        {
            return _client.Contacts.Any(contact => contact.Id == id);
        }
    }
}