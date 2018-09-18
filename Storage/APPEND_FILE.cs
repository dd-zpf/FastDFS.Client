using System;
using System.Net;
using FastDFS.Client.Common;

namespace FastDFS.Client.Storage
{
    /// <summary>
    ///     append file to storage server
    ///     Reqeust
    ///     Cmd: STORAGE_PROTO_CMD_APPEND_FILE 24
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file name length
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: append file body length
    ///     @ file name
    ///     @ append body
    ///     Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:
    /// </summary>
    public class APPEND_FILE : FDFSRequest
    {
        #region 单例

        private static readonly APPEND_FILE _instance = new APPEND_FILE();

        public static APPEND_FILE Instance
        {
            get { return _instance; }
        }

        #endregion

        private APPEND_FILE()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        ///     1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        ///     2,string        FileName
        ///     3,byte[]        File bytes
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
                throw new FDFSException("param count is wrong");
            var endPoint = (IPEndPoint)paramList[0];

            var fileName = (string)paramList[1];
            var contentBuffer = (byte[])paramList[2];

            var result = new APPEND_FILE { Connection = ConnectionManager.GetStorageConnection(endPoint) };

            long length = Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + fileName.Length +
                          contentBuffer.Length;
            var bodyBuffer = new byte[length];

            byte[] fileNameLenBuffer = Util.LongToBuffer(fileName.Length);
            Array.Copy(fileNameLenBuffer, 0, bodyBuffer, 0, fileNameLenBuffer.Length);

            byte[] fileSizeBuffer = Util.LongToBuffer(contentBuffer.Length);
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE, fileSizeBuffer.Length);

            byte[] fileNameBuffer = Util.StringToByte(fileName);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE,
                fileNameBuffer.Length);

            Array.Copy(contentBuffer, 0, bodyBuffer,
                Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE + fileNameBuffer.Length,
                contentBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_APPEND_FILE, 0);
            return result;
        }

        public class Response
        {
            public string FileName;
            public string GroupName;

            public Response(byte[] responseBody)
            {
            }
        }
    }
}