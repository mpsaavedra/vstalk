using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using VSTalk.Engine.Core;
using VSTalk.Engine.Core.Context;
using VSTalk.Engine.Model;
using VSTalk.Engine.Utils.Extensions;
using VSTalk.Tools;
using VSTalk.Model;

namespace VSTalk.Engine.ViewModel
{
    public class InterlocutorChatViewModel : INotifyPropertyChanged
    {
        private Interlocutor interlocutor;

        private RichTextBox chatTextBox;

        private RichTextBox messageTextBox;

        private string message;

        private IModelContext _context;

        #region properties
        public Interlocutor Interlocutor
        {
            get { return interlocutor; }
            set
            {
                interlocutor = value;
                PropertyChanged.Notify(() => Interlocutor);
            }
        }

        public RichTextBox ChatTextBox
        {
            get { return chatTextBox; }
            set
            {
                chatTextBox = value;
                PropertyChanged.Notify(() => ChatTextBox);
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                PropertyChanged.Notify(() => Message);
            }
        }

        public RichTextBox MessageTextBox
        {
            get { return messageTextBox; }
            set
            {
                messageTextBox = value;
                PropertyChanged.Notify(() => MessageTextBox);
            }
        }

        #endregion

        public string XamlMessage
        {
            get
            {
                var xdoc = new XmlDocument();
                xdoc.LoadXml(XamlWriter.Save(MessageTextBox.Document));
                XmlAttribute newAttr = xdoc.CreateAttribute("xml:space");
                newAttr.Value = "preserve";
                xdoc.DocumentElement.Attributes.Append(newAttr);
                StringBuilder sb = new StringBuilder(xdoc.OuterXml);
                //see: http://stackoverflow.com/questions/2624068/wpf-richtextbox-xamlwriter-behaviour
                sb.Replace("{}{", "{");
                return sb.ToString();
            }
        }

        public string TextMessage
        {
            get
            {
                return (new TextRange(
                    MessageTextBox.Document.ContentStart,
                    MessageTextBox.Document.ContentEnd
                    )).Text.TrimEnd(Environment.NewLine.ToCharArray());
            }
        }

        public InterlocutorChatViewModel(Interlocutor interlocutor, IModelContext context)
        {
            Interlocutor = interlocutor;
            _context = context;
            Message = string.Empty;

            ChatTextBox = new RichTextBox(new FlowDocument());
            ChatTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            ChatTextBox.IsReadOnly = true;

            MessageTextBox = new RichTextBox(new FlowDocument());
            MessageTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;


            ParseCurrentHistory();
            Interlocutor.History.CollectionChanged += HistoryChanged;
        }

        public void AppendToMessage(string str)
        {
            MessageTextBox.Document.Blocks.Add(new Paragraph(new Run(str)));
        }

        public void ClearMessage()
        {
            MessageTextBox.Document.Blocks.Clear();
        }

        public bool IsMessageEmpty()
        {
            return new TextRange(MessageTextBox.Document.ContentStart, MessageTextBox.Document.ContentEnd).IsEmpty;
        }

        protected void ParseCurrentHistory()
        {
            ModifyTextBox(() =>
            {
                foreach (var msg in Interlocutor.History)
                {
                    ChatTextBox.Document.Blocks.AddRange(msg.CreateParagraph(_context));
                }
            });
        }

        protected void HistoryChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            ModifyTextBox(() =>
            {
                foreach (var newItem in notifyCollectionChangedEventArgs.NewItems)
                {
                    var msg = newItem as Message;
                    ChatTextBox.Document.Blocks.AddRange(msg.CreateParagraph(_context));
                }
            });
        }

        protected void ModifyTextBox(Action action)
        {
            ChatTextBox.BeginChange();
            action();
            ChatTextBox.EndChange();
            ChatTextBox.ScrollToEnd();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}