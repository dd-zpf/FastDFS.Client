using System.Configuration;

namespace FastDFS.Client.Config
{
    public sealed class FastDfsManager
    {
        public static FastDfsConfig GetConfigSection(string sectionName = "fastdfs")
        {
            return ConfigurationManager.GetSection(sectionName) as FastDfsConfig;
        }
    }
}
