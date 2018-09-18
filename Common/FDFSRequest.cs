using System;
using System.Text;
using System.Threading;

namespace FastDFS.Client.Common
{
    public class FDFSRequest
    {
        #region 公共属性

        public FDFSHeader Header { get; set; }

        public byte[] Body { get; set; }

        public Connection Connection { get; set; }

        #endregion

        #region 公共方法

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 公共虚方法

        public virtual FDFSRequest GetRequest(params object[] paramList)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] GetResponse()
        {
            if (Connection == null)
            {
                Connection = ConnectionManager.GetTrackerConnection();
            }

            try
            {
                //打开
                Connection.OpenConnection();

                var stream = Connection.GetStream();
                var headerBuffer = Header.ToByte();

                stream.Write(headerBuffer, 0, headerBuffer.Length);
                stream.Write(Body, 0, Body.Length);

                var header = new FDFSHeader(stream);
                if (header.Status != 0)
                    throw new FDFSException(string.Format("Get Response Error,Error Code:{0}", header.Status));

                var body = new byte[header.Length];
                if (header.Length != 0)
                {
                    byte[] myReadBuffer = new byte[1024*2];
                    int numberOfBytesRead = 0;
                    int pos = 0;
                    int tatal = (int)header.Length;
                    while (tatal - pos > 0)
                    {
                        numberOfBytesRead = stream.Read(myReadBuffer, 0,Math.Min(myReadBuffer.Length, tatal - pos));
                        Array.Copy(myReadBuffer, 0, body, pos, numberOfBytesRead);
                        pos += numberOfBytesRead;
                    }
                }
                return body;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭
                //Connection.Close();
                Connection.ReleaseConnection();
            }
        }

        #endregion
    }
}