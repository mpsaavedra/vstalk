using System;
using System.Runtime.Serialization;

namespace VSTalk.Engine.Model
{
    [DataContract]
    public class VsJid
    {
        private string _jid;

        [DataMember]
        public string Jid
        {
            get { return _jid; }
            set { _jid = value; }
        }

        #region Equality members

        public override string ToString()
        {
            return _jid;
        }

        protected bool Equals(VsJid other)
        {
            return string.Equals(_jid, other._jid, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VsJid) obj);
        }

        public override int GetHashCode()
        {
            return (_jid != null ? _jid.GetHashCode() : 0);
        }

        public static bool operator ==(VsJid left, VsJid right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VsJid left, VsJid right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
