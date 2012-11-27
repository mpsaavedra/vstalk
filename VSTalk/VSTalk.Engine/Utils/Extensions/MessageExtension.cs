using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using VSTalk.Engine.Core;
using VSTalk.Engine.Core.Context;
using VSTalk.Model;
using agsXMPP;
using VSTalk.Engine.Model;

namespace VSTalk.Engine.Utils.Extensions
{
    public static class MessageExtension
    {
        public static IEnumerable<Paragraph> CreateParagraph(this Message message, IModelContext context)
        {
            var paragraphs = new List<Paragraph>();
            var name = GetName(message, context);
            var title = new Bold(new Run(string.Format("{0} {1:hh:mm}: ", name, message.Date)));

            var paragraph = new Paragraph();
            paragraph.Inlines.Add(title);
            paragraphs.Add(paragraph);

            if (message.Type == MessageType.Xaml)
            {
                var body = ParseXamlString(message.Body);
                paragraphs.AddRange(body.Blocks.Select(block => block as Paragraph));
            }
            else
            {
                var body = new Run(message.Body);
                paragraph.Inlines.Add(body);
            }

            return paragraphs;
        }

        private static string GetName(Message message, IModelContext context)
        {
            var client = context.GetClientById(message.From);
            if (client != null)
            {
                return client.Login;
            }
            var interlocutor = context.GetContactById(message.From);
            if (interlocutor != null)
            {
                return interlocutor.Name;
            }
            return message.From.Jid;
        }

        private static FlowDocument ParseXamlString(string xamlString)
        {
            var stringReader = new StringReader(xamlString);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = false;
            var xmlReader = XmlReader.Create(stringReader, settings);
            return XamlReader.Load(xmlReader) as FlowDocument;
        }
    }
}