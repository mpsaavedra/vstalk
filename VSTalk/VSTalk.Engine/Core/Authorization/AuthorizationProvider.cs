using System.Linq;
using VSTalk.Engine.Core.Messages;
using VSTalk.Engine.Core.Notifications;
using VSTalk.Engine.Core.XmppUtils;
using VSTalk.Engine.Model;
using VSTalk.Model;
using agsXMPP;
using agsXMPP.protocol.client;

namespace VSTalk.Engine.Core.Authorization
{
    public class AuthorizationProvider : IAuthorizationProvider
    {
        private readonly Client _client;
        private readonly INotificationQueue _queue;
        private readonly PresenceManager _presenceManager;

        public AuthorizationProvider(Client client, INotificationQueue queue, XmppClientConnection connection)
        {
            _client = client;
            _queue = queue;
            _presenceManager = new PresenceManager(connection);
        }

        public void HandleAuthorizationsRequest(VsJid id)
        {
            var to = XmppIdConverter.Convert(id);
            var name = GetInterlocutor(id).Name;
            _queue.PushToFront(new AuthorizationRequestMessage(
                                       name,
                                       () => _presenceManager.ApproveSubscriptionRequest(to),
                                       () => _presenceManager.RefuseSubscriptionRequest(to)));
        }

        public void PushAuthorizationsRequest(VsJid id)
        {
            var to = XmppIdConverter.Convert(id);
            _presenceManager.Subscribe(to);
        }

        private Interlocutor GetInterlocutor(VsJid id)
        {
            return _client.Contacts.First(contact => contact.Id == id);
        }
    }
}