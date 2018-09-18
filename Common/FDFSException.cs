using System;

namespace FastDFS.Client.Common
{
    public class FDFSException : Exception
    {
        public FDFSException(string msg) :
            base(msg)
        {

        }
    }
}
