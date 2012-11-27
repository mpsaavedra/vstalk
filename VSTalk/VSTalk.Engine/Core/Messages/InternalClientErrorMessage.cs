using System;
using VSTalk.Engine.Core.Notifications;

namespace VSTalk.Engine.Core.Messages
{
    class InternalClientErrorMessage : INotificationMessage
    {
        public string Title
        {
            get
            {
                return "Error!";
            }
        }

        public string Message { get; private set; }

        public Action ConfirmCallback { get; set; }

        public Action RejectCallback
        {
            get
            {
                return null;
            }
        }

        public InternalClientErrorMessage(string message)
        {
            Message = message;
            ConfirmCallback = () => { };
        }

        public InternalClientErrorMessage(string message, Action confirmCallback)
        {
            Message = message;
            ConfirmCallback = confirmCallback;
        }
    }
}
