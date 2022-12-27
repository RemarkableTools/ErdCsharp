using System;

namespace ErdCsharp.Domain.Exceptions
{
    public class CannotCreateAddressException : Exception
    {
        public CannotCreateAddressException(string input)
            : base($"Cannot create address from {input}") { }
    }
}
