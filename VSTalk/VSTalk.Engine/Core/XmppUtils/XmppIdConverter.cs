using VSTalk.Engine.Model;
using agsXMPP;

namespace VSTalk.Engine.Core.XmppUtils
{
    public static class XmppIdConverter
    {
        public static VsJid Convert(Jid jid)
        {
            return new VsJid
            {
                    Jid = jid.Bare
            };
        }

        public static Jid Convert(VsJid id)
        {
            return new Jid(id.Jid);
        }

    }
}