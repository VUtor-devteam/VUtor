using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.MetaAPI.Exceptions
{
    internal class InvalidClientResponseException : Exception
    {
        public InvalidClientResponseException() { }
        public InvalidClientResponseException(string message) : base(message) { }
        public InvalidClientResponseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
