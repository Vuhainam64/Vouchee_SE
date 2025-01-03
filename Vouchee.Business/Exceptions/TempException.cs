using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Exceptions
{
    public class TempException : Exception
    {
        public TempException(string message) : base(message)
        {
        }
    }
}
