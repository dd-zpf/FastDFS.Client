using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FastDFS.Client.Common;
using FastDFS.Client.Config;

namespace FastDFS.Client
{
    /// <summary>
    /// 链接管理池
    /// </summary>
    public sealed class ConnectionManager
    {
        #region 私有字段

        private static List<IPEndPoint> _listTrackers = new List<IPEndPoint>();

        #endregion

        #region 公共静态字段

        public static Dictionary<IPEndPoint, Pool> TrackerPools = new Dictionary<IPEndPoint, Pool>();
        public static Dictionary<IPEndPoint, Pool> StorePools = new Dictionary<IPEndPoint, Pool>();

        #endregion

        #region 公共静态方法

        public static bool Initialize(List<IPEndPoint> trackers)
        {
            foreach (var point in trackers)
            {
                if (!TrackerPools.ContainsKey(point))
                    TrackerPools.Add(point, new Pool(point, FDFSConfig.TrackerMaxConnection));
            }

            _listTrackers = trackers;

            return true;
        }

        public static bool InitializeForConfigSection(FastDfsConfig config)
        {
            if (config != null)
            {
                var trackers = new List<IPEndPoint>();

                foreach (var ipInfo in config.FastDfsServer)
                {
                    trackers.Add(new IPEndPoint(IPAddress.Parse(ipInfo.IpAddress), ipInfo.Port));
                }

                return Initialize(trackers);
            }

            return false;
        }

        public static Connection GetTrackerConnection()
        {
            var index = new Random().Next(TrackerPools.Count);

            var pool = TrackerPools[_listTrackers[index]];

            return pool.GetConnection();
        }

        public static Connection GetStorageConnection(IPEndPoint endPoint)
        {
            lock ((StorePools as ICollection).SyncRoot)
            {
                if (!StorePools.ContainsKey(endPoint))
                {
                    StorePools.Add(endPoint, new Pool(endPoint, FDFSConfig.StorageMaxConnection));
                }
            }

            return StorePools[endPoint].GetConnection();
        }
        
        #endregion
    }
}