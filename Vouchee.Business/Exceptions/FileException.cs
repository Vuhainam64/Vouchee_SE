﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Exceptions
{
    public class FileException : Exception
    {
        public FileException(string message) : base(message)
        {
        }
    }
}
