using System.Linq;
using VSTalk.Engine.Model;
using VSTalk.Model;

namespace VSTalk.Engine.Core.RosterObserver
{
    public class RosterListener : IRosterListener
    {
        private readonly Client _client;
        public RosterListener(Client client)
        {
            _client = client;
        }

        public void CreateInterlocutor(IRosterInfo info)
        {
            _client.Contacts.Add(new Interlocutor
            {
                    Id = info.Id,
                    Name = info.Name,
                    Subscribed = info.Subscribed,
                    ImName = info.ImName,
                    State = ContactState.Offline,
                    HasUnreadMessages = false
            });
        }

        public void UpdateInterlocutor(IRosterInfo info)
        {
            var targetContact = _client.Contacts.First(contact => contact.Id == info.Id);

            targetContact.Name = info.Name;
            targetContact.ImName = info.ImName;
            targetContact.Subscribed = info.Subscribed;
        }

        public void RemoveInterlocutor(VsJid id)
        {
            var targetContact = _client.Contacts.First(contact => contact.Id == id);
            _client.Contacts.Remove(targetContact);
        }
    }
}