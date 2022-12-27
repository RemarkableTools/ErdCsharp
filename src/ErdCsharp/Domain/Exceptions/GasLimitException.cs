using System;
using System.Collections.Generic;
using System.Text;

namespace ErdCsharp.Domain.Exceptions
{
    public class GasLimitException
    {
        public class UndefinedGasLimitException : Exception
        {
            public UndefinedGasLimitException()
                : base("Gas limit is undefined") { }
        }

        public class NotEnoughGasException : Exception
        {
            public NotEnoughGasException(string message)
                : base(message) { }
        }
    }
}
