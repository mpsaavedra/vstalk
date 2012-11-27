using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using VSTalk.Engine.Core.Authorization;
using VSTalk.Engine.Core.Messages;
using VSTalk.Engine.Core.Notifications;
using VSTalk.Engine.Core.PresenceObserver;
using VSTalk.Engine.Core.RosterObserver;
using VSTalk.Engine.Core.XmppUtils;
using VSTalk.Model;
using agsXMPP;
using agsXMPP.Net;
using agsXMPP.protocol.iq.roster;
using VSTalk.Tools;
using Message = VSTalk.Engine.Model.Message;
using MessageType = VSTalk.Model.MessageType;

namespace VSTalk.Engine.Core
{
    public class ClientConnection : IClientService
    {
        private readonly Client _client;

        private XmppClientConnection _connection = new XmppClientConnection(SocketConnectionType.Direct);
        
        private IAuthorizationProvider _authorizationProvider;

        private readonly RosterManager _rosterManager;

        private readonly INotificationQueue _notificationQueue;
        
        internal Client Client
        {
            get { return _client; }
        }

        public const string JABBER_SERVER = "jabber.org";

        public const string RESOURCE_NAME = "VsTalk";

        public ClientConnection(INotificationQueue notificationQueue, Client client)
        {
            _client = client;
            _notificationQueue = notificationQueue;

            _rosterManager = new RosterManager(_connection);

            SubscribeToEvents();

            SetAuthorizationProvider();
            SetRosterObsererver();
            SetPresenceObserver();

            HandleClientChanges();
        }

        private void SetAuthorizationProvider()
        {
            _authorizationProvider = new AuthorizationProvider(_client, _notificationQueue, _connection);
        }

        private void SetPresenceObserver()
        {
            var listener = new PresenceListener(_client, _authorizationProvider);
            new PresenceObserver.PresenceObserver(_connection, listener);
        }

        private void SetRosterObsererver()
        {
            var listener = new RosterListener(_client);
            var container = new RosterContainer(_client);
            new RosterObesrver(_connection, listener, container);
        }

        private void HandleClientChanges()
        {
            Client.SubscribeToChange(() => Client.Enabled, delegate
            {
                if (Client.Enabled && Client.State == ClientState.Disconnected)
                {
                    Connect();
                }
            });
        }

        private void SubscribeToEvents()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            _connection.OnError += dispatcher.Wrap<object, Exception>(OnError).Exec;
            _connection.OnLogin += dispatcher.Wrap<object>(OnLogin).Exec;
            _connection.OnAuthError += dispatcher.Wrap<object, agsXMPP.Xml.Dom.Element>(OnAuthError).Exec;
            _connection.OnMessage += dispatcher.Wrap<object, agsXMPP.protocol.client.Message>(OnMessage).Exec;
            _connection.OnXmppConnectionStateChanged += dispatcher.Wrap<object, XmppConnectionState>(OnXmppConnectionStateChanged).Exec;
#if DEBUG
            _connection.OnReadXml += (sender, xml) => Debug.WriteLine(string.Format("RECV: {0}", xml));
            _connection.OnWriteXml += (sender, xml) => Debug.WriteLine(string.Format("SEND: {0}", xml));
#endif
        }

        private void OnXmppConnectionStateChanged(object sender, XmppConnectionState state)
        {
            if (state == XmppConnectionState.Disconnected)
            {
                Client.State = ClientState.Disconnected;
            }
        }

        internal void Connect()
        {
            Debug.Assert(_client.State != ClientState.Connected, "Already connected");
            _client.State = ClientState.Connecting;
            _connection.Username = _client.Login;
            _connection.Password = _client.Password;
            _connection.Resource = RESOURCE_NAME;
            _connection.Server = string.IsNullOrEmpty(_client.Server)
                ? JABBER_SERVER
                : _client.Server;
            _connection.UseSSL = _client.UseSsl;
            _connection.RegisterAccount = false;
            _connection.AutoResolveConnectServer = true;
            _connection.UseCompression = false;
            _connection.EnableCapabilities = true;

            _connection.Open();
        }

        protected void OnError(object sender, Exception ex)
        {
            Debug.WriteLine(ex);
            _notificationQueue.PushToFront(new InternalClientErrorMessage(string.Format("{0} : {1}", _client.Login, ex.Message)));
            
            Client.State = ClientState.Disconnected;
            _connection.Close();
        }

        internal void Disconnect()
        {
            _connection.Close();
        }

        public bool HasContact(Interlocutor interlocutor)
        {
            return Client.Contacts.Any(contact => contact == interlocutor);
        }

        public void RemoveContact(Interlocutor interlocutor)
        {
            _rosterManager.RemoveRosterItem(XmppIdConverter.Convert(interlocutor.Id));
            //not waiting responce after RemoveRosterItem executed
            Client.Contacts.Remove(interlocutor);
        }

        public void SendMessage(Interlocutor interlocutor, Message message)
        {
            var outMessage = new agsXMPP.protocol.client.Message();
            outMessage.To = XmppIdConverter.Convert(interlocutor.Id);
            outMessage.Body = message.Body;
            outMessage.Type = agsXMPP.protocol.client.MessageType.chat;
            _connection.Send(outMessage);

            message.From = Client.Id;
            message.To = interlocutor.Id;
            message.Date = DateTime.Now;

            interlocutor.History.Add(message);
        }

        public void AddContact(string contactName)
        {
            _rosterManager.AddRosterItem(new Jid(contactName, _client.Server, null));
        }

        public void SendRequestAuthorization(Interlocutor interlocutor)
        {
            _authorizationProvider.PushAuthorizationsRequest(interlocutor.Id);
        }

        #region HANDLING internalClient EVENTS
        private void OnAuthError(object sender, agsXMPP.Xml.Dom.Element rp)
        {
            Debug.WriteLine("Auth Error" + rp);
            var message = string.Format("Authentification failed {0}!", Client.Login);
            _notificationQueue.PushToFront(new InternalClientErrorMessage(message));
            Disconnect();
        }

        private void OnLogin(object sender)
        {
            _client.State = ClientState.Connected;
            _client.Id = XmppIdConverter.Convert(_connection.MyJID);
        }

        #endregion


        #region HANDLING rosterManager & presenceManager EVENTS

        private void OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            var interlocutor = _client.Contacts.SingleOrDefault(contact => contact.Id == XmppIdConverter.Convert(msg.From));
            if (interlocutor == null || string.IsNullOrEmpty(msg.Body)) return;
            interlocutor.History.Add(new Message
            {
                Body = msg.Body,
                Date = DateTime.Now,
                From = interlocutor.Id,
                To = Client.Id,
                Type = interlocutor.ImName == RESOURCE_NAME ? MessageType.Xaml : MessageType.Text
            });
            interlocutor.HasUnreadMessages = true;
        }
        #endregion
    }
}