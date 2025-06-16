// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;

#pragma warning disable IDE0130 // folder structure
#pragma warning disable S101 // pascal naming convention.
// spell-checker:ignore stdf

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class PCR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private readonly StdfValueConverter _valueConverter;
        public override Stdf4RecordType Stdf4RecordType { get; } = Stdf4RecordType.PCR;
        public override string RecordName { get; } = "PCR";
        public override byte RecordType { get; } = 1;
        public override byte RecordSubtype { get; } = 30;

        public byte TestHeadNumber { get; set; }
        public byte TestSiteNumber { get; set; }
        public uint NumberPartsTested { get; set; }
        public uint NumberPartsRetested { get; set; }
        public uint NumberAbortsDuringTesting { get; set; }
        public uint NumberGoodParts { get; set; }
        public uint NumberFunctionalParts { get; set; }

        public PCR(StdfValueConverter converter)
        {
            this._valueConverter = converter;
        }

        public PCR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public PCR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   Tested {this.NumberPartsTested}    Good {this.NumberGoodParts}    Functional {this.NumberFunctionalParts}";

        public PCR Parse(byte[] recordData, byte cpuType = 2)
        {
            return this.Parse(recordData.AsSpan(), offset: 0, cpuType: cpuType);
        }

        public PCR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            // Console.WriteLine("PCR: Have " + recordData.Length + " bytes.");

            this.TestHeadNumber = recordData[offset];
            offset += 1;

            this.TestSiteNumber = recordData[offset];
            offset += 1;

            this.NumberPartsTested = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberPartsRetested = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberAbortsDuringTesting = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberGoodParts = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberFunctionalParts = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length() => 26;


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

            idx += this._valueConverter.SetUint32(this.NumberPartsTested, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberPartsRetested, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberAbortsDuringTesting, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberGoodParts, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberFunctionalParts, destinationByteArray, offset + idx);

            // Lastly set the length, which is the first field so no + idx with the offset nor to accumulate length.
            this._valueConverter.SetUint16(idx, destinationByteArray, offset: offset);
            return idx;
        }

    }
}
