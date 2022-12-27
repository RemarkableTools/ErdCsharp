using System;

namespace ErdCsharp.Domain.Exceptions
{
    public class BinaryCodecException : Exception
    {
        public BinaryCodecException(string message)
            : base(message) { }
    }
}
