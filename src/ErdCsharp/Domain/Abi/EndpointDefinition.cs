﻿using ErdCsharp.Domain.Values;

namespace ErdCsharp.Domain.Abi
{
    public class EndpointDefinition
    {
        public string            Name   { get; }
        public FieldDefinition[] Input  { get; }
        public FieldDefinition[] Output { get; }

        public EndpointDefinition(string name, FieldDefinition[] input, FieldDefinition[] output)
        {
            Name   = name;
            Input  = input;
            Output = output;
        }
    }
}
