using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using VSTalk.Engine.Model;
using System.Linq;
using VSTalk.Model;
using VsTalk.Model;

namespace VSTalk.Engine.Settings
{
    public class MessageRepository
    {
        private static readonly string settingFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private const string CLIENT_STORAGE_FILE_NAME = "vstalk_history.xml";

        private static readonly string filePath = Path.Combine(settingFolder, CLIENT_STORAGE_FILE_NAME);

        public History LoadHistory(Interlocutor interlocutor)
        {
            return LoadFullHistory().FirstOrDefault(note => interlocutor.Id == note.InterlocutorId);
        }

        private IEnumerable<History> LoadFullHistory()
        {
            var history = new List<History>();
            if (!File.Exists(filePath))
            {
                return history;
            }
            var fs = new FileStream(filePath, FileMode.Open);
            try
            {
                var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                var ser = new DataContractSerializer(history.GetType());

                history = (List<History>)ser.ReadObject(reader, true);

                reader.Close();
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Cannot deserialize history file:", e.ToString());
            }
            finally
            {
                fs.Close();
            }
            return history;
        }

        public void SaveHistory(Account account)
        {
            var interlocutors = account.XmppClients.SelectMany(client => client.Contacts);
            var storeHistory = interlocutors.Select(interlocutor => new History
            {
                InterlocutorId = interlocutor.Id,
                Messages = interlocutor.History.ToList()
            });

            var fullHistory = LoadFullHistory().ToList();

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            foreach (var note in storeHistory)
            {
                var contactHistory = fullHistory.FirstOrDefault(h => h.InterlocutorId == note.InterlocutorId);
                if (contactHistory != null)
                {
                    contactHistory.Messages.AddRange(note.Messages
                        .Where(msg => contactHistory.Messages
                        .All(hMsg => hMsg.Date != msg.Date)));
                }
                else
                {
                    fullHistory.Add(note);
                }
            }

            var writer = new FileStream(filePath, FileMode.Create);
            var ser = new DataContractSerializer(fullHistory.GetType());
            try
            {
                ser.WriteObject(writer, fullHistory);
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Cannot serialize history file:", e.ToString());
            }
            finally
            {
                writer.Close();
            }
        }
    }
}