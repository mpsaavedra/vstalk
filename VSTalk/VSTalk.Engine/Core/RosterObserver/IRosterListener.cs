using VSTalk.Engine.Model;

namespace VSTalk.Engine.Core.RosterObserver
{
    public interface IRosterListener
    {
        void CreateInterlocutor(IRosterInfo info);
        void UpdateInterlocutor(IRosterInfo info);
        void RemoveInterlocutor(VsJid id);
    }
}