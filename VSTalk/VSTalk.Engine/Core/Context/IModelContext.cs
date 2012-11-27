using System.Collections.ObjectModel;
using VSTalk.Engine.Model;
using VSTalk.Model;
using VsTalk.Model;

namespace VSTalk.Engine.Core.Context
{
    public interface IModelContext
    {
        Account Account { get; }
        ObservableCollection<Client> Clients { get; }

        
        Client GetClientById(VsJid id);
        Interlocutor GetContactById(VsJid id);
        Client GetClientByContact(Interlocutor interlocutor);
    }
}