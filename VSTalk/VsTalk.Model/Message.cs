using System;
using System.Runtime.Serialization;
using VSTalk.Model;

namespace VSTalk.Engine.Model
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string Body { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public VsJid From { get; set; }

        [DataMember]
        public VsJid To { get; set; }

        [DataMember]
        public MessageType Type { get; set; }
    }
}
