using VSTalk.Engine.Model;

namespace VSTalk.Engine.Core.RosterObserver
{
    public interface IRosterInfo
    {
        VsJid Id { get; }
        string Name { get; }
        string ImName { get; }
        bool Subscribed { get; }
    }
}