using System.Runtime.Serialization;

namespace Message
{
    [Serializable]
    [DataContract]
    public class Message
    {
        [DataMember]
        public string? message { get; set; }
        [DataMember]
        public string? user { get; set; }
        [DataMember]
        public bool Disconnect { get; set; }
        [DataMember]
        public bool Connect { get; set; }
    }
}
