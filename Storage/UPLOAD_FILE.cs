using System;
using System.Net;
using System.Net.Sockets;
using FastDFS.Client.Common;

namespace FastDFS.Client.Storage
{
    /// <summary>
    ///     upload file to storage server
    ///     Reqeust
    ///     Cmd: STORAGE_PROTO_CMD_UPLOAD_FILE 11
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename size
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file bytes size
    ///     @ filename
    ///     @ file bytes: file content
    ///     Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// </summary>
    public class UPLOAD_FILE : FDFSRequest
    {
        private static readonly UPLOAD_FILE _instance = new UPLOAD_FILE();

        private UPLOAD_FILE()
        {
        }

        public static UPLOAD_FILE Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        ///     1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        ///     2,Byte          StorePathIndex
        ///     3,long          FileSize
        ///     4,string        File Ext
        ///     5,byte[FileSize]    File Content
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 5)
                throw new FDFSException("param count is wrong");

            var endPoint = (IPEndPoint)paramList[0];

            var storePathIndex = (byte)paramList[1];
            var fileSize = (int)paramList[2];
            var ext = (string)paramList[3];
            var contentBuffer = (byte[])paramList[4];

            #region 拷贝后缀扩展名值

            var extBuffer = new byte[Consts.FDFS_FILE_EXT_NAME_MAX_LEN];
            byte[] bse = Util.StringToByte(ext);
            int extNameLen = bse.Length;
            if (extNameLen > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
            {
                extNameLen = Consts.FDFS_FILE_EXT_NAME_MAX_LEN;
            }
            Array.Copy(bse, 0, extBuffer, 0, extNameLen);

            #endregion

            var result = new UPLOAD_FILE
            {
                Connection = ConnectionManager.GetStorageConnection(endPoint)
            };

            if (ext.Length > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
                throw new FDFSException("file ext is too long");

            long length = 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN + contentBuffer.Length;
            var bodyBuffer = new byte[length];
            bodyBuffer[0] = storePathIndex;

            byte[] fileSizeBuffer = Util.LongToBuffer(fileSize);

            Array.Copy(fileSizeBuffer, 0, bodyBuffer, 1, fileSizeBuffer.Length);

            Array.Copy(extBuffer, 0, bodyBuffer, 1 + Consts.FDFS_PROTO_PKG_LEN_SIZE, extBuffer.Length);

            Array.Copy(contentBuffer, 0, bodyBuffer,
                1 + Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_FILE_EXT_NAME_MAX_LEN, contentBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FDFSHeader(length, Consts.STORAGE_PROTO_CMD_UPLOAD_FILE, 0);

            return result;
        }

        public class Response
        {
            public string FileName;
            public string GroupName;

            public Response(byte[] responseBody)
            {
                var groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
                Array.Copy(responseBody, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
                GroupName = Util.ByteToString(groupNameBuffer).TrimEnd('\0');

                var fileNameBuffer = new byte[responseBody.Length - Consts.FDFS_GROUP_NAME_MAX_LEN];
                Array.Copy(responseBody, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer, 0, fileNameBuffer.Length);
                FileName = Util.ByteToString(fileNameBuffer).TrimEnd('\0');
            }
        }
    }
}