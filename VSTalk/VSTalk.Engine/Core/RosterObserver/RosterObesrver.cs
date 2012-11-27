using System.Diagnostics;
using System.Windows.Threading;
using VSTalk.Engine.Core.XmppUtils;
using VSTalk.Engine.Model;
using VSTalk.Tools;
using agsXMPP;
using agsXMPP.protocol.iq.roster;

namespace VSTalk.Engine.Core.RosterObserver
{
    public class RosterObesrver
    {
        private readonly IRosterListener _listener;
        private readonly IRosterContainer _container;
        private readonly XmppClientConnection _connection;

        private readonly Dispatcher _dispatcher; 

        public RosterObesrver(XmppClientConnection connection, IRosterListener listener, IRosterContainer container)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            _connection = connection;
            _listener = listener;
            _container = container;

            SetHandlers();
        }

        private void SetHandlers()
        {
            _connection.OnRosterStart += _dispatcher.Wrap<object>(OnRosterStart).Exec;

            _connection.OnRosterItem += _dispatcher.Wrap<object, RosterItem>(OnRosterItem).Exec;

            _connection.OnRosterEnd += _dispatcher.Wrap<object>(OnRosterEnd).Exec;
        }

        private void OnRosterStart(object sender)
        {
            Debug.WriteLine("Reciving Roster {0}", _connection.MyJID);
        }

        private void OnRosterItem(object sender, RosterItem item)
        {
            var vsId = XmppIdConverter.Convert(item.Jid);

            if (_container.Contains(vsId) && item.Subscription == SubscriptionType.remove)
            {
                _listener.RemoveInterlocutor(vsId);
            }
            else if (_container.Contains(vsId) && item.Subscription != SubscriptionType.remove)
            {
                _listener.UpdateInterlocutor(RosterInfo.Create(item));
            }
            else if (!_container.Contains(vsId) && item.Subscription != SubscriptionType.remove)
            {
                _listener.CreateInterlocutor(RosterInfo.Create(item));
            }
        }

        private void OnRosterEnd(object sender)
        {
            Debug.WriteLine("Roster recived {0}", _connection.MyJID);
        }

        private class RosterInfo : IRosterInfo
        {
            public VsJid Id { get; private set; }
            public string Name { get; private set; }
            public string ImName { get; private set; }
            public bool Subscribed { get; private set; }

            public static RosterInfo Create(RosterItem item)
            {
                bool isSubscribed = item.Subscription == SubscriptionType.both ||
                                    item.Subscription == SubscriptionType.to;
                var name = string.IsNullOrEmpty(item.Name) ? item.Jid.User : item.Name;
                var id = XmppIdConverter.Convert(item.Jid);
                return new RosterInfo
                {
                        Id = id,
                        Name =  name,
                        ImName = item.Jid.Resource,
                        Subscribed = isSubscribed
                };
            }
        }
    }
}