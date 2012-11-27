namespace VSTalk.Engine.Core.Notifications
{
    public interface INotificationQueue
    {
        void PushToFront(INotificationMessage message);
        void PushBack(INotificationMessage message);
    }
}