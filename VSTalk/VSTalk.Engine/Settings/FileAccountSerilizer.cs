using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using VSTalk.Engine.Core.Context;
using VSTalk.Engine.Model;
using VSTalk.Tools;
using VSTalk.Model;
using VsTalk.Model;

namespace VSTalk.Engine.Settings
{
    public class FileAccountSerilizer : IAccountSerilizer, IAccountDeserilizer
    {
        private static readonly string settingFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private const string CLIENT_STORAGE_FILE_NAME = "vstalk_clients.xml";

        private static readonly string filePath = Path.Combine(settingFolder, CLIENT_STORAGE_FILE_NAME);

        private Account DeserializeFile()
        {
            var account = new Account();
            if (!File.Exists(filePath))
            {
                return account;
            }
            var fs = new FileStream(filePath, FileMode.Open);
            try
            {
                var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                var ser = new DataContractSerializer(account.GetType());

                // Deserialize the data and read it from the instance.
                account = (Account)ser.ReadObject(reader, true);
                
                reader.Close();
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Cannot deserialize contact list", e.ToString());
            }
            finally
            {
                fs.Close();
            }

            return account;
        }

        private void SerilizeToFile(Account account)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            var writer = new FileStream(filePath, FileMode.Create);
            var ser = new DataContractSerializer(account.GetType());
            try
            {
                ser.WriteObject(writer, account);
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Cannot serialize contact list", e.ToString());
            }
            finally
            {
                writer.Close();                
            }
        }

        public void Serilize(Account account)
        {
            SerilizeToFile(account);
        }

        public Account Deserilize()
        {
            return DeserializeFile();
        }
    }
}