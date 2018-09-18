using System;
using System.Net;
using FastDFS.Client.Common;

namespace FastDFS.Client.Storage
{
    /// <summary>
    ///     query file info from storage server
    ///     Reqeust
    ///     Cmd: STORAGE_PROTO_CMD_QUERY_FILE_INFO 22
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    ///     Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file size
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file create timestamp
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file CRC32 signature
    /// </summary>
    public class QUERY_FILE_INFO : FDFSRequest
    {
        private static readonly QUERY_FILE_INFO _instance = new QUERY_FILE_INFO();

        private QUERY_FILE_INFO()
        {
        }

        public static QUERY_FILE_INFO Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        ///     1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        ///     2,string fileName
        ///     3,string fileBytes
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
                throw new FDFSException("param count is wrong");
            var endPoint = (IPEndPoint) paramList[0];

            var groupName = (string) paramList[1];
            var fileName = (string) paramList[2];

            var result = new QUERY_FILE_INFO();
            result.Connection = ConnectionManager.GetStorageConnection(endPoint);

            if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
                throw new FDFSException("groupName is too long");

            long length = Consts.FDFS_GROUP_NAME_MAX_LEN + fileName.Length;
            var bodyBuffer = new byte[length];
            byte[] groupNameBuffer = Util.StringToByte(groupName);
            byte[] fileNameBuffer = Util.StringToByte(fileName);

            Array.Copy(groupNameBuffer, 0, bodyBuffer, 0, groupNameBuffer.Length);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_QUERY_FILE_INFO, 0);
            return result;
        }
    }
}