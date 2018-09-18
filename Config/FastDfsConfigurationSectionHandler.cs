using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FastDFS.Client.Config
{
    /// <summary>
    /// 默认SectionName：fastdfs
    /// <FastDfsConfig GroupName="group1">
    ///     <FastDfsServer IpAddress="127.0.0.1" Port="11211" />
    ///     <FastDfsServer IpAddress="192.168.0.1" Port="11211" />
    /// </FastDfsConfig>
    /// </summary>
    public class FastDfsConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(FastDfsConfig));
                using (var stream = new MemoryStream(Encoding.Default.GetBytes(section.InnerXml)))
                {
                    var xmlNode = XmlReader.Create(stream);
                    return xmlSerializer.Deserialize(xmlNode);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}