using VSTalk.Model;
using agsXMPP.protocol.client;
using VSTalk.Engine.Model;

namespace VSTalk.Engine.Utils.Extensions
{
    public static class PresenceExtension
    {
        //Todo: CHECK ALL POSIBLE STATE
        //see: http://code.google.com/p/jabber-net/source/browse/trunk/muzzle/RosterTree.cs#674
        public static ContactState ParseState(this Presence presence)
        {
            if (presence != null && presence.Type == PresenceType.available)
            {
                if (presence.Show == ShowType.away)
                {
                    return ContactState.Away;
                }
                return ContactState.Online;
            }
            return ContactState.Offline;
        }
    }
}