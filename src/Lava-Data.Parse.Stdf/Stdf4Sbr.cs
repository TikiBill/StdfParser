// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class SBR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "SBR";
        public override byte RecordType { get; } = 1;
        public override byte RecordSubtype { get; } = 50;

        public byte TestHeadNumber { get; set; }
        public byte TestSiteNumber { get; set; }
        public ushort SoftwareBinNumber { get; set; }
        public uint NumberOfPartsInBin { get; set; }
        public char PassFailIndicator { get; set; }
        public string SoftwareBinName { get; set; }

        public SBR() { }

        public SBR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public SBR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   SWBin {this.SoftwareBinNumber}    PartsInBin {this.NumberOfPartsInBin}    Pass/Fail {this.PassFailIndicator}    Name {this.SoftwareBinName}";

        public SBR Parse(byte[] recordData, byte cpuType = 2)
        {
            return this.Parse(recordData.AsSpan(), offset: 0, cpuType: cpuType);
        }

        public SBR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            this.TestHeadNumber = recordData[offset];
            offset += 1;

            this.TestSiteNumber = recordData[offset];
            offset += 1;

            this.SoftwareBinNumber = this._valueConverter.GetUInt16AndUpdateOffset(recordData, ref offset);
            this.NumberOfPartsInBin = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);


            this.PassFailIndicator = (char)recordData[offset];
            offset += 1;

            this.SoftwareBinName = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            ushort length = 13;
            if (this.SoftwareBinName == null)
            {
                // No change to record length. Not going to even be in the record.
                // Also, short-circuits checking other properties.
                return length;
            }
            else if (this.SoftwareBinName.Length > 255)
            {
                throw new Stdf4ParserException("SBR.SoftwareBinName is too long! Max allowed is 255 characters.");
            }
            else
            {
                // The extra byte is for the length of the string.
                length += (ushort)(this.SoftwareBinName.Length + 1);
            }
            return length;
        }


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

            idx += this._valueConverter.SetUint32(this.NumberOfPartsInBin, destinationByteArray, offset + idx);

            destinationByteArray[offset + idx] = Convert.ToByte(this.PassFailIndicator);

            if (this.SoftwareBinName != null)
            {
                idx += this._valueConverter.WriteAsciiString(this.SoftwareBinName, destinationByteArray, offset + idx);
            }

            // Lastly set the length.
            idx += this._valueConverter.SetUint16(idx, destinationByteArray, offset); // No + idx! Goes in first location.
            return idx;
        }

    }
}
