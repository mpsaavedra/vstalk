using System.Linq;
using VSTalk.Engine.Core.Authorization;
using VSTalk.Engine.Model;
using VSTalk.Model;

namespace VSTalk.Engine.Core.PresenceObserver
{
    public class PresenceListener : IPersenceListener
    {
        private readonly Client _client;
        private readonly IAuthorizationProvider _provider;

        public PresenceListener(Client client, IAuthorizationProvider provider)
        {
            _client = client;
            _provider = provider;
        }

        public void ChangedState(VsJid id, ContactState state)
        {
            GetInterlocutorOrSelf(id).State = state;
        }

        public void UpdatedImName(VsJid id, string name)
        {
            GetInterlocutorOrSelf(id).ImName = name;
        }

        public void RequestsAuthorization(VsJid id)
        {
            _provider.HandleAuthorizationsRequest(id);
        }

        private IContact GetInterlocutorOrSelf(VsJid id)
        {
            if (_client.Id == id) return new ClientStub(_client);
            return new InterlocutorStub(_client.Contacts.First(contact => contact.Id == id));
        }

        private interface IContact
        {
            ContactState State { get; set; }
            string ImName { get; set; }
        }

        private class InterlocutorStub : IContact
        {
            private readonly Interlocutor _interlocutor;
            public InterlocutorStub(Interlocutor interlocutor)
            {
                _interlocutor = interlocutor;
            }

            public ContactState State
            {
                get { return _interlocutor.State; }
                set { _interlocutor.State = value; }
            }

            public string ImName
            {
                get { return _interlocutor.ImName; }
                set { _interlocutor.ImName = value; }
            }
        }

        private class ClientStub : IContact
        {
            private readonly Client _client;
            public ClientStub(Client client)
            {
                _client = client;
            }

            public ContactState State
            {
                get
                {
                    if (_client.State == ClientState.Connected || _client.State == ClientState.Connected)
                    {
                        return ContactState.Online;
                    }
                    return ContactState.Offline;
                }
                set { }
            }

            public string ImName
            {
                get { return ClientConnection.RESOURCE_NAME; }
                set { }
            }
        }
    }    
}