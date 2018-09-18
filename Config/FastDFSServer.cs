using System;
using System.Xml.Serialization;

namespace FastDFS.Client.Config
{
    [Serializable]
    public class FastDfsServer
    {
        [XmlAttribute]
        public string IpAddress { get; set; }

        [XmlAttribute]
        public int Port { get; set; }
    }
}