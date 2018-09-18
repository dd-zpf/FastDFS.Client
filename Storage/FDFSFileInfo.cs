using System;
using FastDFS.Client.Common;

namespace FastDFS.Client.Storage
{
    public class FDFSFileInfo
    {
        public long Crc32;
        public DateTime CreateTime;
        public long FileSize;

        public FDFSFileInfo(byte[] responseByte)
        {
            var fileSizeBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            var createTimeBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            var crcBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];

            Array.Copy(responseByte, 0, fileSizeBuffer, 0, fileSizeBuffer.Length);
            Array.Copy(responseByte, Consts.FDFS_PROTO_PKG_LEN_SIZE, createTimeBuffer, 0, createTimeBuffer.Length);
            Array.Copy(responseByte, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE, crcBuffer, 0,
                crcBuffer.Length);

            FileSize = Util.BufferToLong(responseByte, 0);
            CreateTime =
                new DateTime(1970, 1, 1).AddSeconds(Util.BufferToLong(responseByte, Consts.FDFS_PROTO_PKG_LEN_SIZE));

            Crc32 = Util.BufferToLong(responseByte, Consts.FDFS_PROTO_PKG_LEN_SIZE + Consts.FDFS_PROTO_PKG_LEN_SIZE);
        }
    }
}
