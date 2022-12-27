using System;

namespace ErdCsharp.Domain.Exceptions
{
    public class InsufficientFundException : Exception
    {
        public InsufficientFundException(string tokenIdentifier)
            : base($"Insufficient fund for token : {tokenIdentifier}") { }
    }
}
