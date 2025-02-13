// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// spell-checker:ignore stdf
namespace LavaData.Parse.Stdf4
{
    [Serializable]
    public class Stdf4ParserException : Exception
    {
        public Stdf4ParserException()
        {
        }

        public Stdf4ParserException(string message) : base(message)
        {
        }

        public Stdf4ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Stdf4ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
