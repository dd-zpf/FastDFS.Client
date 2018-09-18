using System;
using System.IO;

namespace FastDFS.Client.Common
{
    public class FDFSHeader
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="length"></param>
        /// <param name="command"></param>
        /// <param name="status"></param>
        public FDFSHeader(long length, byte command, byte status)
        {
            Length = length;
            Command = command;
            Status = status;
        }

        public FDFSHeader(Stream stream)
        {
            var headerBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE + 2];
            int bytesRead = stream.Read(headerBuffer, 0, headerBuffer.Length);

            if (bytesRead == 0)
                throw new FDFSException("Init Header Exeption : Cann't Read Stream");

            Length = Util.BufferToLong(headerBuffer, 0);
            Command = headerBuffer[Consts.FDFS_PROTO_PKG_LEN_SIZE];
            Status = headerBuffer[Consts.FDFS_PROTO_PKG_LEN_SIZE + 1];
        }

        /// <summary>
        ///     Pachage Length
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        ///     Command
        /// </summary>
        public byte Command { get; set; }

        /// <summary>
        ///     Status
        /// </summary>
        public byte Status { get; set; }

        public byte[] ToByte()
        {
            var result = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE + 2];
            byte[] pkglen = Util.LongToBuffer(Length);
            Array.Copy(pkglen, 0, result, 0, pkglen.Length);
            result[Consts.FDFS_PROTO_PKG_LEN_SIZE] = Command;
            result[Consts.FDFS_PROTO_PKG_LEN_SIZE + 1] = Status;
            return result;
        }
    }
}