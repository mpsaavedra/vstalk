using System.Collections.Generic;
using System.Runtime.Serialization;
using VSTalk.Engine.Model;

namespace VSTalk.Model
{
    [DataContract]
    public class History
    {
        [DataMember]
        public VsJid InterlocutorId { get; set; }

        [DataMember]
        public List<Message> Messages { get; set; }
    }
}
