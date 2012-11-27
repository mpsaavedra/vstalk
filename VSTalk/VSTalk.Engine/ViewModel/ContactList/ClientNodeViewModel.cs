using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using VSTalk.Model;
using VSTalk.Tools;

namespace VSTalk.Engine.ViewModel.ContactList
{
    public class ClientNodeViewModel
    {
        public Client Client { get; private set; }

        public ObservableCollection<Interlocutor> Interlocutors { get; private set; }

        private bool ShowOffline
        {
            get { return Properties.Settings.Default.ShowOfflineContacts; }
        }

        public ClientNodeViewModel(Client client)
        {
            Client = client;

            Interlocutors = new ObservableCollection<Interlocutor>();

            HandleCollectionChanges();
            HandleSettingsChanges();

            PopulateContacts();
        }

        private void PopulateContacts()
        {
            foreach (var contact in Client.Contacts)
            {
                SubscribeToChange(contact);
                AddInterlocutor(contact);
            }
        }

        private void HandleSettingsChanges()
        {
            Properties.Settings.Default.SettingsSaving += delegate
            {
                if (ShowOffline)
                {
                    ShowOfflineContact();
                }
                else
                {
                    RemoveOfflineContacts();
                }
            };
        }

        private void HandleCollectionChanges()
        {
            Client.Contacts.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (var newItem in args.NewItems)
                    {
                        var newContact = (Interlocutor)newItem;
                        SubscribeToChange(newContact);
                        AddInterlocutor(newContact);
                    }
                }
                if (args.OldItems != null)
                {
                    foreach (var oldItem in args.OldItems)
                    {
                        var oldContact = (Interlocutor)oldItem;
                        UnsubscribeFromChange(oldContact);
                        RemoveInterlocutor((Interlocutor)oldItem);
                    }
                }
            };
        }

        private void UnsubscribeFromChange(Interlocutor oldContact)
        {
            oldContact.PropertyChanged -= InterlocutorStateChanged;
        }

        private void SubscribeToChange(Interlocutor newContact)
        {
            newContact.PropertyChanged += InterlocutorStateChanged;
        }

        private void ShowOfflineContact()
        {
            var offlineContacts = Client.Contacts.Where(interlocutor => interlocutor.State == ContactState.Offline).ToList();
            foreach (var offlineContact in offlineContacts)
            {
                AddInterlocutor(offlineContact);
            }
        }

        private void RemoveOfflineContacts()
        {
            var offlineContacts = Interlocutors.Where(interlocutor => interlocutor.State == ContactState.Offline).ToList();
            foreach (var offlineContact in offlineContacts)
            {
                RemoveInterlocutor(offlineContact);
            }
        }

        private void AddInterlocutor(Interlocutor interlocutor)
        {
            if (!CanAddInterlocutor(interlocutor)) return;
            int targetIndex = InterlocutorPosition(interlocutor);
            Interlocutors.Insert(targetIndex, interlocutor);
        }

        private int InterlocutorPosition(Interlocutor interlocutor)
        {
            var interlocutors = Interlocutors.AsEnumerable();
            if (!interlocutors.Contains(interlocutor))
            {
                interlocutors = interlocutors.Concat(new[] { interlocutor });
            }
            return interlocutors.OrderBy(contact => contact.State).ThenBy(contact => contact.Name).ToList().IndexOf(interlocutor);
        }

        private void InterlocutorStateChanged(object sender, PropertyChangedEventArgs e)
        {
            var interlocutor = (Interlocutor)sender;
            if (e.PropertyName == "State")
            {
                if (!ShowOffline && interlocutor.State == ContactState.Offline && Interlocutors.Contains(interlocutor))
                {
                    RemoveInterlocutor(interlocutor);
                }
                else if (interlocutor.State != ContactState.Offline && !Interlocutors.Contains(interlocutor))
                {
                    AddInterlocutor(interlocutor);
                }
                else
                {
                    UpdatePlacement(interlocutor);
                }
            }
        }

        private void UpdatePlacement(Interlocutor interlocutor)
        {
            var targetPosition = InterlocutorPosition(interlocutor);
            var actualPosition = Interlocutors.IndexOf(interlocutor);
            if (targetPosition == actualPosition) return;
            RemoveInterlocutor(interlocutor);
            AddInterlocutor(interlocutor);
        }

        private bool CanAddInterlocutor(Interlocutor interlocutor)
        {
            return ShowOffline || interlocutor.State != ContactState.Offline;
        }

        private void RemoveInterlocutor(Interlocutor interlocutor)
        {
            Interlocutors.Remove(interlocutor);
        }
    }
}