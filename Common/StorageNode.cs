using System.Net;

namespace FastDFS.Client.Common
{
    public class StorageNode
    {
        public string GroupName;
        public IPEndPoint EndPoint;
        public byte StorePathIndex;
    }
}
