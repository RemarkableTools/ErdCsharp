﻿using System.Collections.Generic;
using System.Linq;
using ErdCsharp.Domain.Exceptions;
using ErdCsharp.Domain.Values;

namespace ErdCsharp.Domain.Codec
{
    public class BinaryCodec
    {
        private readonly List<IBinaryCodec> _codecs;

        public BinaryCodec()
        {
            _codecs = new List<IBinaryCodec>
            {
                new NumericBinaryCodec(),
                new AddressBinaryCodec(),
                new BooleanBinaryCodec(),
                new BytesBinaryCodec(),
                new MultiBinaryCodec(this),
                new TokenIdentifierCodec(),
                new StructBinaryCodec(this),
                new OptionBinaryCodec(this),
            };
        }

        public (IBinaryType Value, int BytesLength) DecodeNested(byte[] data, TypeValue type)
        {
            CheckBufferLength(data);

            var codec = _codecs.SingleOrDefault(c => c.Type == type.BinaryType);
            if (codec == null)
                throw new BinaryCodecException($"No codec found for {type.BinaryType}");

            var decode = codec.DecodeNested(data, type);
            return decode;
        }

        public IBinaryType DecodeTopLevel(byte[] data, TypeValue type)
        {
            CheckBufferLength(data);

            var codec = _codecs.SingleOrDefault(c => c.Type == type.BinaryType);
            if (codec == null)
                throw new BinaryCodecException($"No codec found for {type.BinaryType}");

            var decode = codec.DecodeTopLevel(data, type);
            return decode;
        }

        public byte[] EncodeNested(IBinaryType value)
        {
            var codec = _codecs.SingleOrDefault(c => c.Type == value.Type.BinaryType);
            if (codec == null)
                throw new BinaryCodecException($"No codec found for {value.Type.BinaryType}");

            var encode = codec.EncodeNested(value);
            return encode;
        }

        public byte[] EncodeTopLevel(IBinaryType value)
        {
            var codec = _codecs.SingleOrDefault(c => c.Type == value.Type.BinaryType);
            if (codec == null)
                throw new BinaryCodecException($"No codec found for {value.Type.BinaryType}");

            var encode = codec.EncodeTopLevel(value);
            return encode;
        }

        private static void CheckBufferLength(byte[] buffer)
        {
            if (buffer.Length > 4096)
            {
                throw new BinaryCodecException("Buffer too large");
            }
        }
    }
}
