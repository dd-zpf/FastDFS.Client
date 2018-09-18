using System;
using FastDFS.Client.Common;

namespace FastDFS.Client.Tracker
{
    /// <summary>
    /// query which storage server to store file
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE 104
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    ///     @ 1 byte: store path index on the storage server
    /// </summary>
    public class QUERY_STORE_WITH_GROUP_ONE : FDFSRequest
    {
        #region 单例对象
        
        private static readonly QUERY_STORE_WITH_GROUP_ONE _instance = new QUERY_STORE_WITH_GROUP_ONE();

        public static QUERY_STORE_WITH_GROUP_ONE Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramList">
        /// 1,string groupName-->the storage groupName
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length == 0)
                throw new FDFSException("GroupName is null");

            var result = new QUERY_STORE_WITH_GROUP_ONE();

            var groupName = Util.StringToByte((string)paramList[0]);
            if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
            {
                throw new FDFSException("GroupName is too long");
            }

            var body = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(groupName, 0, body, 0, groupName.Length);
            result.Body = body;
            result.Header = new FDFSHeader(Consts.FDFS_GROUP_NAME_MAX_LEN, Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE, 0);

            return result;
        }

        public class Response
        {
            public string GroupName;
            public string IpStr;
            public int Port;
            public byte StorePathIndex;
            public Response(byte[] responseByte)
            {
                var groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];

                Array.Copy(responseByte, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);

                GroupName = Util.ByteToString(groupNameBuffer).TrimEnd('\0');

                var ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];

                Array.Copy(responseByte, Consts.FDFS_GROUP_NAME_MAX_LEN, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);

                IpStr = new string(FDFSConfig.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');

                var portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];

                Array.Copy(responseByte, Consts.FDFS_GROUP_NAME_MAX_LEN + Consts.IP_ADDRESS_SIZE - 1, portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);

                Port = (int)Util.BufferToLong(portBuffer, 0);

                StorePathIndex = responseByte[responseByte.Length - 1];
            }
        }
    }
}