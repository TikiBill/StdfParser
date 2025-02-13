// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class FAR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "FAR";
        public override byte RecordType { get; } = 0;
        public override byte RecordSubtype { get; } = 10;

        public byte CpuType { get; set; } = BitConverter.IsLittleEndian ? (byte)2 : (byte)1;

        public byte StdfVersion { get; set; } = 4;

        public FAR() { }

        public FAR(StdfValueConverter converter)
        {
            this._valueConverter = converter;
        }

        public FAR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public FAR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"FAR   CPU Type {this.CpuType}    StdfVersion {this.StdfVersion}";

        public FAR Parse(ReadOnlySpan<byte> recordData, int offset = 0)
        {
            if (recordData.Length - offset >= 2)
            {
                this.CpuType = recordData[offset + 0];
                this.StdfVersion = recordData[offset + 1];
            }
            else
            {
                throw new Stdf4ParserException("Need at least two bytes for a FAR record, only got " + recordData.Length);
            }
            return this;
        }

        public FAR Parse(byte[] recordData, int offset = 0)
        {
            return this.Parse(recordData.AsSpan(), offset);
        }

        /// <inheritdoc/>
        public override ushort Length() => 6; // Always 6 bytes, including the record length.


        /// <inheritdoc/>
        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {
            (destinationByteArray[offset + 0], destinationByteArray[offset + 1]) =
                this._valueConverter.UshortToBytes(2); // Only two non-header bytes written.
            destinationByteArray[offset + 2] = this.RecordType;
            destinationByteArray[offset + 3] = this.RecordSubtype;
            destinationByteArray[offset + 4] = this.CpuType;
            destinationByteArray[offset + 5] = this.StdfVersion;
            return 6; // Bytes written
        }

    }
}
