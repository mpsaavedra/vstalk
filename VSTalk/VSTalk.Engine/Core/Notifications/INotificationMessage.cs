using System;

namespace VSTalk.Engine.Core.Notifications
{
    public interface INotificationMessage
    {
        string Title { get; }

        string Message { get; }

        Action ConfirmCallback { get; }

        Action RejectCallback { get; }
    }
}
