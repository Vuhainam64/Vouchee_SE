﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Exceptions
{
    public class InvalidFieldException : Exception
    {
        public InvalidFieldException(string message) : base(message)
        {
        }
    }
}