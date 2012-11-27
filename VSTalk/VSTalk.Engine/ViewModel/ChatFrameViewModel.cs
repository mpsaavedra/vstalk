using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VSTalk.Engine.Core;
using VSTalk.Engine.Model;
using VSTalk.Engine.Utils;
using VSTalk.Tools;
using VSTalk.Model;

namespace VSTalk.Engine.ViewModel
{
    public class ChatFrameViewModel : INotifyPropertyChanged
    {
        private readonly VSTalkCore _core;

        public ObservableCollection<InterlocutorChatViewModel> ViewContacts { get; set; }

        private InterlocutorChatViewModel _selectedInterlocutor;

        public RelayCommand SendMessage { get; private set; }

        public RelayCommand ShowMainControl { get; private set; }

        public RelayCommand PasteCallStack { get; private set; }

        public RelayCommand PasteActiveDocument { get; private set; }

        public RelayCommand PasteDebugOutput { get; private set; }
        
        public RelayCommand<InterlocutorChatViewModel> CloseInterlocutorTab { get; private set; }

        public InterlocutorChatViewModel SelectedInterlocutor
        {
            get { return _selectedInterlocutor; }
            set
            {
                _selectedInterlocutor = value;
                PropertyChanged.Notify(() => SelectedInterlocutor);
            }
        }

        public ChatFrameViewModel(VSTalkCore core)
        {
            _core = core;

            SetCommands();

            ViewContacts = new ObservableCollection<InterlocutorChatViewModel>();
            this.SubscribeToChange(() => SelectedInterlocutor,
                param =>
                {
                    if (SelectedInterlocutor != null)
                    {
                        SelectedInterlocutor.Interlocutor.HasUnreadMessages = false;
                    }
                });
        }

        private void SetCommands()
        {
            ShowMainControl = new RelayCommand(ShowMainWindowExecuted);
            PasteCallStack = new RelayCommand(PasteCallStackExecuted);
            PasteActiveDocument = new RelayCommand(PasteActiveDocumentExecuted);
            PasteDebugOutput = new RelayCommand(PasteDebugOutputExecuted);

            SendMessage = new RelayCommand(SendMessageExecuted, SendMessageCanExecute);

            CloseInterlocutorTab = new RelayCommand<InterlocutorChatViewModel>(CloseInterlocutorTabExecuted);
        }

        private void ShowMainWindowExecuted()
        {
            _core.ShowMainWindow();
        }

        protected void PasteDebugOutputExecuted()
        {
            var doc = _core.EnvironmentManager.GetDebugOutput();
            if (string.IsNullOrEmpty(doc)) return;
            SelectedInterlocutor.AppendToMessage(string.Format("========Debug Output========\n{0}", doc));
        }

        protected void PasteActiveDocumentExecuted()
        {
            var doc = _core.EnvironmentManager
                .GetActiveDocument();
            if (string.IsNullOrEmpty(doc)) return;
            SelectedInterlocutor.AppendToMessage(string.Format("========Active Document========\n{0}", doc));
        }

        protected void PasteCallStackExecuted()
        {
            var currentStack = _core.EnvironmentManager
                .GetCallStack()
                .Reverse()
                .ToList();

            if (currentStack.Count() == 0) return;
            var sb = new StringBuilder();
            currentStack.Each(frame => sb.Append(frame + "\n"));
            SelectedInterlocutor.AppendToMessage(string.Format("\n========Call Stack========\n{0}", sb));
        }

        protected void CloseInterlocutorTabExecuted(InterlocutorChatViewModel interlocutorModel)
        {
            ViewContacts.Remove(interlocutorModel);
            SelectedInterlocutor = ViewContacts.FirstOrDefault();
        }

        protected void SendMessageExecuted()
        {
            var interlocutor = _selectedInterlocutor.Interlocutor;
            string body;
            MessageType messageType;
            
            if (interlocutor.ImName == ClientConnection.RESOURCE_NAME)
            {
                body = _selectedInterlocutor.XamlMessage;
                messageType = MessageType.Xaml;
            }
            else
            {
                body = _selectedInterlocutor.TextMessage;
                messageType = MessageType.Text;
            }

            _core.GetClientService(interlocutor).SendMessage(interlocutor,
                                                            new Message
                                                            {
                                                                    Body = body,
                                                                    Type = messageType
                                                            });
            _selectedInterlocutor.ClearMessage();
        }

        protected bool SendMessageCanExecute()
        {
            if (_selectedInterlocutor == null) return false;
            
            var client = _core.ModelContext.GetClientByContact(_selectedInterlocutor.Interlocutor);
            
            if (client == null) return false;

            return _selectedInterlocutor != null
                   && client.State == ClientState.Connected
                   && !_selectedInterlocutor.IsMessageEmpty();

        }

        public void AppendInterlocutor(Interlocutor interlocutor)
        {
            if (ViewContacts.Any(contact => contact.Interlocutor == interlocutor))
            {
                SelectedInterlocutor = ViewContacts.Single(contact => contact.Interlocutor == interlocutor);
                return;
            }
            var interlocutorModel = new InterlocutorChatViewModel(interlocutor, _core.ModelContext);
            ViewContacts.Add(interlocutorModel);
            SelectedInterlocutor = interlocutorModel;
            interlocutor.SubscribeToChange(() => interlocutor.HasUnreadMessages, sender =>
            {
                if (SelectedInterlocutor != null && SelectedInterlocutor.Interlocutor == sender &&
                    sender.HasUnreadMessages)
                {
                    sender.HasUnreadMessages = false;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
