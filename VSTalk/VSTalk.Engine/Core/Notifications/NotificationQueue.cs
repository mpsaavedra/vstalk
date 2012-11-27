using System.Collections.ObjectModel;

namespace VSTalk.Engine.Core.Notifications
{
    public class NotificationQueue : INotificationQueue
    {
        public ObservableCollection<INotificationMessage> Messages { get; private set; }

        public NotificationQueue()
        {
            Messages = new ObservableCollection<INotificationMessage>();
        }

        public void Remove(INotificationMessage notification)
        {
            Messages.Remove(notification);
        }

        public void PushToFront(INotificationMessage message)
        {
            Messages.Insert(0, message);
        }

        public void PushBack(INotificationMessage message)
        {
            Messages.Add(message);
        }
    }
}
