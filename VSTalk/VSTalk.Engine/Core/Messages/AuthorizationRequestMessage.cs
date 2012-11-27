using System;
using VSTalk.Engine.Core.Notifications;

namespace VSTalk.Engine.Core.Messages
{
    class AuthorizationRequestMessage : INotificationMessage
    {
        public string Title
        {
            get
            {
                return "Authorize contact?";
            }
        }

        public string Message { get; set; }

        public Action ConfirmCallback { get; set; }

        public Action RejectCallback { get; set; }

        public AuthorizationRequestMessage(string contactName, Action confirmCallback, Action rejectCallback)
        {
            Message = string.Format("{0} wants to add you to his or her buddy list.", contactName);
            ConfirmCallback = confirmCallback;
            RejectCallback = rejectCallback;
        }
    }
}