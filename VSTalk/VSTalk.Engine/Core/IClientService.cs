using VSTalk.Engine.Model;
using VSTalk.Model;

namespace VSTalk.Engine.Core
{
    public interface IClientService
    {
        void AddContact(string contactName);

        void RemoveContact(Interlocutor interlocutor);

        void SendMessage(Interlocutor interlocutor, Message message);

        void SendRequestAuthorization(Interlocutor interlocutor);
    }
}