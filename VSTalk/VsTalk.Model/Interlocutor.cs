using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using VSTalk.Engine.Model;
using VSTalk.Tools;

namespace VSTalk.Model
{
    [DataContract]
    public class Interlocutor : INotifyPropertyChanged
    {
        private ObservableCollection<Message> history;

        private VsJid id;

        private string name;

        private string _groupName;

        private ContactState state;

        private string status;

        private bool subscribed;

        private bool hasUnreadMessages;

        private string imName;

        #region PROPERTIES

        public ObservableCollection<Message> History
        {
            get { return history; }
            set
            {
                history = value;
                PropertyChanged.Notify(() => History);
            }
        }

        [DataMember]
        public string ImName
        {
            get { return imName; }
            set
            {
                imName = value;
                PropertyChanged.Notify(() => ImName);
            }
        }

        [DataMember]
        public VsJid Id
        {
            get { return id; }
            set
            {
                id = value;
                PropertyChanged.Notify(() => Id);
            }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                PropertyChanged.Notify(() => Name);
            }
        }

        [DataMember]
        public string GroupName
        {
            get { return _groupName; }
            set
            {
                _groupName = value;
                PropertyChanged.Notify(() => GroupName);
            }
        }

        [DataMember]
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                PropertyChanged.Notify(() => Status);
            }
        }

        [DataMember]
        public bool HasUnreadMessages
        {
            get { return hasUnreadMessages; }
            set
            {
                hasUnreadMessages = value;
                PropertyChanged.Notify(() => HasUnreadMessages);
            }
        }

        [DataMember]
        public bool Subscribed
        {
            get { return subscribed; }
            set
            {
                subscribed = value;
                if (!subscribed)
                {
                    State = ContactState.Offline;
                }
                PropertyChanged.Notify(() => Subscribed);
            }
        }

        public ContactState State
        {
            get { return state; }
            set
            {
                state = value;
                PropertyChanged.Notify(() => State);
            }
        }

        #endregion

        public Interlocutor()
        {
            History = new ObservableCollection<Message>();
            State = ContactState.Offline;
        }

        [OnDeserializing]
        internal void OnDeserializing(StreamingContext context)
        {
            if (History == null)
            {
                History = new ObservableCollection<Message>();
            }
            State = ContactState.Offline;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
