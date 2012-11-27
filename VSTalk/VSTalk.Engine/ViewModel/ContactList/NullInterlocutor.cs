using System.ComponentModel;
using System.Linq;
using VSTalk.Model;
using VSTalk.Tools;

namespace VSTalk.Engine.ViewModel.ContactList
{
    public class NullInterlocutor : INotifyPropertyChanged
    {
        private readonly Client _client;

        private string _notice;
        public string Notice
        {
            get { return _notice; }
            private set
            {
                _notice = value;
                PropertyChanged.Notify(() => Notice);
            }
        }
        
        public NullInterlocutor(Client client)
        {
            _client = client;
            _client.Contacts.CollectionChanged += (sender, args) => UpdateNotice();
            UpdateNotice();
        }

        private void UpdateNotice()
        {
            if (_client.Contacts.Count == 0)
            {
                Notice = string.Format("no contacts");
                return;
            }
            if (_client.Contacts.All(contact => contact.State == ContactState.Offline))
            {
                Notice = string.Format("none online");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}