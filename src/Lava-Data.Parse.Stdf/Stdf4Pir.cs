// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class PIR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "PIR";
        public override byte RecordType { get; } = 5;
        public override byte RecordSubtype { get; } = 10;

        public byte TestHeadNumber { get; set; }
        public byte TestSiteNumber { get; set; }

        public PIR() { }

        public PIR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public PIR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   Head Number {this.TestHeadNumber}    Site Number {this.TestSiteNumber}";

        public PIR Parse(byte[] recordData)
        {
            return this.Parse(recordData.AsSpan(), offset: 0);
        }

        public PIR Parse(ReadOnlySpan<byte> recordData, int offset = 0)
        {
            int idx = 0;

            this.TestHeadNumber = recordData[offset + idx];
            idx += 1;

            this.TestSiteNumber = recordData[offset + idx];
            idx += 1;

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length() => 6;


        /// <inheritdoc/>
        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {
            ushort idx = 2; // SKipping the first two length bytes.

            destinationByteArray[offset + idx] = this.RecordType;
            idx += 1;

            destinationByteArray[offset + idx] = this.RecordSubtype;
            idx += 1;

            destinationByteArray[offset + idx] = this.TestHeadNumber;
            idx += 1;

            destinationByteArray[offset + idx] = this.TestSiteNumber;
            idx += 1;

            // Lastly set the length, which is the first field so no + idx with the offset nor to accumulate length.
            this._valueConverter.SetUint16(idx, destinationByteArray, offset: offset);
            return idx;
        }

    }
}
