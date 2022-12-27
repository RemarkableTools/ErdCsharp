using System;

namespace ErdCsharp.Domain.Exceptions
{
    public class WrongBinaryValueCodecException : Exception
    {
        public WrongBinaryValueCodecException()
            : base("Wrong binary argument") { }
    }
}
