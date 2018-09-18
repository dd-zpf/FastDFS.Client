using System;
using FastDFS.Client.Common;

namespace FastDFS.Client.Tracker
{
    /// <summary>
    ///     query which storage server to download the file
    ///     Reqeust
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ONE 102
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    ///     Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes:  storage server ip address
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    /// </summary>
    public class QUERY_FETCH_ONE : FDFSRequest
    {
        private static readonly QUERY_FETCH_ONE _instance = new QUERY_FETCH_ONE();

        private QUERY_FETCH_ONE()
        {
        }

        public static QUERY_FETCH_ONE Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        ///     1,string groupName
        ///     2,string fileName
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 2)
                throw new FDFSException("param count is wrong");

            var result = new QUERY_FETCH_ONE();
            var groupName = (string) paramList[0];
            var fileName = (string) paramList[1];
            if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
                throw new FDFSException("GroupName is too long");

            byte[] groupNameBuffer = Util.StringToByte(groupName);
            byte[] fileNameBuffer = Util.StringToByte(fileName);
            int length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileNameBuffer.Length;
            var body = new byte[length];

            Array.Copy(groupNameBuffer, 0, body, 0, groupNameBuffer.Length);
            Array.Copy(groupNameBuffer, 0, body, 0, groupNameBuffer.Length);

            result.Body = body;
            result.Header = new FDFSHeader(length,
                Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_FETCH_ONE, 0);
            return result;
        }

        public class Response
        {
            public string GroupName;
            public string IpStr;
            public int Port;

            public Response(byte[] responseByte)
            {
                var groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
                Array.Copy(responseByte, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
                GroupName = Util.ByteToString(groupNameBuffer).TrimEnd('\0');
                var ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];
                Array.Copy(responseByte, Consts.FDFS_GROUP_NAME_MAX_LEN, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);
                IpStr = new string(FDFSConfig.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');
                var portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
                Array.Copy(responseByte, Consts.FDFS_GROUP_NAME_MAX_LEN + Consts.IP_ADDRESS_SIZE - 1,
                    portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);
                Port = (int) Util.BufferToLong(portBuffer, 0);
            }
        }
    }
}