using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignAndEncyptTool
{
    public class SAEException : Exception
    {
        public SAEException(string? message) : base(message) { }
        public SAEException(string? message, Exception innerException) : base(message, innerException) { }
    }
}
