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
    public class WIR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private readonly StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "WIR";
        public override byte RecordType { get; } = 2;
        public override byte RecordSubtype { get; } = 10;

        public byte TestHeadNumber { get; set; }
        public byte SiteGroupNumber { get; set; }
        public uint FirstPartTestedTs { get; set; }
        public string? WaferId { get; set; }


        public WIR(StdfValueConverter converter){
            this._valueConverter = converter;
        }

        public WIR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public WIR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"WIR   Wafer {this.WaferId}    TestHead {this.TestHeadNumber}    SiteGroup {this.SiteGroupNumber}";

        public WIR Parse(byte[] recordData)
        {
            return this.Parse(recordData.AsSpan(), offset: 0);
        }

        public WIR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            this.TestHeadNumber = recordData[offset];
            offset += 1;

            this.SiteGroupNumber = recordData[offset];
            offset += 1;

            this.FirstPartTestedTs = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.WaferId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            ushort length = 10;
            if (this.WaferId == null)
            {
                // No change to record length. Not going to even be in the record.
            }
            else if (this.WaferId.Length > 255)
            {
                throw new Stdf4ParserException("WIR.WaferId is too long! Max allowed is 255 characters.");
            }
            else
            {
                // The extra byte is for the length of the string.
                length += (ushort)(this.WaferId.Length + 1);
            }
            return length;
        }


        /// <inheritdoc/>
        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {

            ushort idx = 2; // Skip length.

            destinationByteArray[offset + idx] = this.RecordType;
            idx += 1;

            destinationByteArray[offset + idx] = this.RecordSubtype;
            idx += 1;

            destinationByteArray[offset + idx] = this.TestHeadNumber;
            idx += 1;

            destinationByteArray[offset + idx] = this.SiteGroupNumber;
            idx += 1;


            idx += this._valueConverter.SetUint32(this.FirstPartTestedTs, destinationByteArray, offset + idx);


            if (this.WaferId != null)
            {
                idx += this._valueConverter.WriteAsciiString(this.WaferId, destinationByteArray, offset + idx);
            }

            // Lastly set the length, which is the first field so no + idx with the offset nor to accumulate length.
            this._valueConverter.SetUint16(idx, destinationByteArray, offset: offset);
            return idx;
        }

    }
}
