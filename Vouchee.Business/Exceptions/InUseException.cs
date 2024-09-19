using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Exceptions
{
    public class InUseException : Exception
    {
        public InUseException(string message) : base(message)
        {
        }
    }
}