using System.Net;
using FastDFS.Client.Common;
using FastDFS.Client.Storage;
using FastDFS.Client.Tracker;

namespace FastDFS.Client
{
    /// <summary>
    /// FastDFSClient
    /// </summary>
    public class FastDFSClient
    {
        #region 公共静态方法

        /// <summary>
        /// 获取存储节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>存储节点实体类</returns>
        public static StorageNode GetStorageNode(string groupName)
        {
            var trackerRequest = QUERY_STORE_WITH_GROUP_ONE.Instance.GetRequest(groupName);

            var trackerResponse = new QUERY_STORE_WITH_GROUP_ONE.Response(trackerRequest.GetResponse());

            var storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IpStr), trackerResponse.Port);

            var result = new StorageNode
            {
                GroupName = trackerResponse.GroupName,
                EndPoint = storeEndPoint,
                StorePathIndex = trackerResponse.StorePathIndex
            };

            return result;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public static string UploadFile(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            var storageReqeust = UPLOAD_FILE.Instance.GetRequest(storageNode.EndPoint, storageNode.StorePathIndex, contentByte.Length, fileExt, contentByte);

            var storageResponse = new UPLOAD_FILE.Response(storageReqeust.GetResponse());

            return storageResponse.FileName;
        }

        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="masterFilename">主文件名</param>
        /// <param name="prefixName">从文件后缀</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public static string UploadSlaveFile(string groupName, byte[] contentByte, string masterFilename, string prefixName, string fileExt)
        {
            var trackerRequest = QUERY_UPDATE.Instance.GetRequest(groupName, masterFilename);

            var trackerResponse = new QUERY_UPDATE.Response(trackerRequest.GetResponse());

            var storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IpStr), trackerResponse.Port);

            var storageReqeust = UPLOAD_SLAVE_FILE.Instance.GetRequest(storeEndPoint, contentByte.Length, masterFilename, prefixName, fileExt, contentByte);

            var storageResponse = new UPLOAD_FILE.Response(storageReqeust.GetResponse());

            return storageResponse.FileName;
        }

        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public static string UploadAppenderFile(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            var storageReqeust = UPLOAD_APPEND_FILE.Instance.GetRequest(storageNode.EndPoint, storageNode.StorePathIndex, contentByte.Length, fileExt, contentByte);

            var storageResponse = new UPLOAD_APPEND_FILE.Response(storageReqeust.GetResponse());

            return storageResponse.FileName;
        }

        /// <summary>
        /// 附加文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentByte">文件内容</param>
        public static void AppendFile(string groupName, string fileName, byte[] contentByte)
        {
            var trackerRequest = QUERY_UPDATE.Instance.GetRequest(groupName, fileName);

            var trackerResponse = new QUERY_UPDATE.Response(trackerRequest.GetResponse());

            var storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IpStr), trackerResponse.Port);

            var storageReqeust = APPEND_FILE.Instance.GetRequest(storeEndPoint, fileName, contentByte);

            storageReqeust.GetResponse();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileName">文件名</param>
        public static void RemoveFile(string groupName, string fileName)
        {
            var trackerRequest = QUERY_UPDATE.Instance.GetRequest(groupName, fileName);

            var trackerResponse = new QUERY_UPDATE.Response(trackerRequest.GetResponse());

            var storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IpStr), trackerResponse.Port);

            var storageReqeust = DELETE_FILE.Instance.GetRequest(storeEndPoint, groupName, fileName);

            storageReqeust.GetResponse();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <returns>文件内容</returns>
        public static byte[] DownloadFile(StorageNode storageNode, string fileName)
        {
            var storageReqeust = DOWNLOAD_FILE.Instance.GetRequest(storageNode.EndPoint, 0L, 0L, storageNode.GroupName, fileName);

            var storageResponse = new DOWNLOAD_FILE.Response(storageReqeust.GetResponse());

            return storageResponse.Content;
        }

        /// <summary>
        /// 增量下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <param name="offset">从文件起始点的偏移量</param>
        /// <param name="length">要读取的字节数</param>
        /// <returns>文件内容</returns>
        public static byte[] DownloadFile(StorageNode storageNode, string fileName, long offset, long length)
        {
            var storageReqeust = DOWNLOAD_FILE.Instance.GetRequest(storageNode.EndPoint, offset, length, storageNode.GroupName, fileName);

            var storageResponse = new DOWNLOAD_FILE.Response(storageReqeust.GetResponse());

            return storageResponse.Content;
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static FDFSFileInfo GetFileInfo(StorageNode storageNode, string fileName)
        {
            var storageReqeust = QUERY_FILE_INFO.Instance.GetRequest(storageNode.EndPoint, storageNode.GroupName, fileName);

            var result = new FDFSFileInfo(storageReqeust.GetResponse());

            return result;
        }

        #endregion
    }
}
