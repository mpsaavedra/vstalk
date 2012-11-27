using VSTalk.Engine.Model;
using VSTalk.Model;

namespace VSTalk.Engine.Core.PresenceObserver
{
    public interface IPersenceListener
    {
        void ChangedState(VsJid id, ContactState state);
        void UpdatedImName(VsJid id, string name);
        void RequestsAuthorization(VsJid id);
    }
}