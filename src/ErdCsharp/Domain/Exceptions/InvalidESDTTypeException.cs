using System;
using System.Collections.Generic;
using System.Text;

namespace ErdCsharp.Domain.Exceptions
{
    public class InvalidESDTTypeException : Exception
    {
        public InvalidESDTTypeException(string value)
            : base($"Invalid ESDT Type {value}") { }
    }
}
