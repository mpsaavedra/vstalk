using VSTalk.Engine.Model;

namespace VSTalk.Engine.Core.RosterObserver
{
    public interface IRosterContainer
    {
        bool Contains(VsJid id);
    }
}