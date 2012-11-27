using System.Windows.Threading;
using VSTalk.Engine.Core.XmppUtils;
using VSTalk.Engine.Utils.Extensions;
using VSTalk.Tools;
using agsXMPP;
using agsXMPP.protocol.client;

namespace VSTalk.Engine.Core.PresenceObserver
{
    public class PresenceObserver
    {
        private XmppClientConnection _connection;

        private IPersenceListener _listener;

        private Dispatcher _dispatcher;

        public PresenceObserver(XmppClientConnection connection, IPersenceListener listener)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            _connection = connection;
            _listener = listener;

            SetHandlers();
        }

        private void SetHandlers()
        {
            _connection.OnPresence += _dispatcher.Wrap<object,Presence>(OnPresence).Exec;
        }

        private void OnPresence(object sender, Presence pres)
        {
            var from = XmppIdConverter.Convert(pres.From);
             
            _listener.ChangedState(from, pres.ParseState());
            
            if (!string.IsNullOrEmpty(pres.From.Resource))
            {
                _listener.UpdatedImName(from, pres.From.Resource);
            }
            
            if (pres.Type == PresenceType.subscribe)
            {
                _listener.RequestsAuthorization(from);
            }
            
        }
    }
}