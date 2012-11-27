using System.ComponentModel;
using System.Windows.Input;
using VSTalk.Engine.Core;
using VSTalk.Engine.Core.Notifications;
using VSTalk.Engine.Utils;
using VSTalk.Engine.Utils.Extensions;
using VSTalk.Tools;

namespace VSTalk.Engine.ViewModel
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        public NotificationQueue Notifications { get; private set; }

        private INotificationMessage _currentNotification;

        public INotificationMessage CurrentNotification
        {
            get { return _currentNotification; }
            set
            {
                _currentNotification = value;
                PropertyChanged.Notify(() => CurrentNotification);
            }
        }

        private int _currentMessageIndex;

        public int CurrentMessageIndex
        {
            get { return _currentMessageIndex; }
            set
            {
                _currentMessageIndex = value;
                PropertyChanged.Notify(() => CurrentMessageIndex);
                if (Notifications.Messages.Count > 0)
                    CurrentNotification = Notifications.Messages[CurrentMessageIndex - 1];
            }
        }

        private bool _hasNotifications;

        public bool HasNotifications
        {
            get
            {
                return _hasNotifications;
            }
            set
            {
                _hasNotifications = value;
                PropertyChanged.Notify(() => HasNotifications);
            }
        }

        public RelayCommand GoToNextMessage { get; private set; }

        public RelayCommand GoToPrevMessage { get; private set; }

        public RelayCommand ConfirmCommand { get; private set; }

        public RelayCommand RejectCommand { get; private set; }

        public RelayCommand HideControl { get; private set; }

        public NotificationViewModel(NotificationQueue notifications)
        {
            Notifications = notifications;
            CurrentMessageIndex = 1;
            HasNotifications = Notifications.Messages.Count > 0;
            Notifications.Messages.CollectionChanged += delegate
            {
                HasNotifications = Notifications.Messages.Count > 0;

                CommandManager.InvalidateRequerySuggested();
                SetCurrentNotification();
            };

            GoToNextMessage = new RelayCommand(GoToNextMessageExecuted, GoToNextMessageCanExecute);
            GoToPrevMessage = new RelayCommand(GoToPrevMessageExecuted, GoToPrevMessageCanExecute);

            ConfirmCommand = new RelayCommand(ConfirmCommandExecuted, ConfirmCommandCanExecute);
            RejectCommand = new RelayCommand(RejectCommandExecuted, RejectCommandCanExecute);

            HideControl = new RelayCommand(HideControlExecuted);
        }

        private void SetCurrentNotification()
        {
            if (CurrentMessageIndex > Notifications.Messages.Count)
                CurrentMessageIndex--;
            if (CurrentMessageIndex == 0 && Notifications.Messages.Count > 0)
                CurrentMessageIndex = 1;
            CurrentNotification = Notifications.Messages.Count > 0 
                ? Notifications.Messages[CurrentMessageIndex - 1] 
                : null;
        }

        private void RejectCommandExecuted()
        {
            CurrentNotification.RejectCallback();
            Notifications.Remove(CurrentNotification);
            SetCurrentNotification();
        }

        private bool RejectCommandCanExecute()
        {
            return  CurrentNotification != null && CurrentNotification.RejectCallback != null;
        }

        private void ConfirmCommandExecuted()
        {
            CurrentNotification.ConfirmCallback();
            Notifications.Remove(CurrentNotification);
            SetCurrentNotification();
        }

        private bool ConfirmCommandCanExecute()
        {
            return CurrentNotification != null && CurrentNotification.ConfirmCallback != null;
        }

        private void HideControlExecuted()
        {
            HasNotifications = false;
        }

        private void GoToPrevMessageExecuted()
        {
            CurrentMessageIndex--;
        }

        private bool GoToPrevMessageCanExecute()
        {
            return CurrentMessageIndex > 1;
        }

        private void GoToNextMessageExecuted()
        {
            CurrentMessageIndex++;
        }

        private bool GoToNextMessageCanExecute()
        {
            return CurrentMessageIndex < Notifications.Messages.Count;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}