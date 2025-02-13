// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class WRR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "WRR";
        public override byte RecordType { get; } = 2;
        public override byte RecordSubtype { get; } = 10;

        public byte TestHeadNumber { get; set; }
        public byte SiteGroupNumber { get; set; }
        public uint LastPartTestedTs { get; set; }
        public uint NumberPartsTested { get; set; }
        public uint NumberPartsRetested { get; set; }
        public uint NumberAbortsDuringTesting { get; set; }
        public uint NumberGoodParts { get; set; }
        public uint NumberFunctionalParts { get; set; }
        public string WaferId { get; set; }
        public string FabWaferId { get; set; }
        public string WaferFrameId { get; set; }
        public string WaferMaskId { get; set; }
        public string WaferDescriptionUser { get; set; }
        public string WaferDescriptionExec { get; set; }

        public WRR() { }

        public WRR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public WRR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   Wafer {this.WaferId}    Frame {this.WaferFrameId}    Mask {this.WaferMaskId}";

        public WRR Parse(byte[] recordData, byte cpuType = 2)
        {
            return this.Parse(recordData.AsSpan(), offset: 0, cpuType: cpuType);
        }

        public WRR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            this.TestHeadNumber = recordData[offset];
            offset += 1;

            this.SiteGroupNumber = recordData[offset];
            offset += 1;

            this.LastPartTestedTs = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberPartsTested = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberPartsRetested = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberAbortsDuringTesting = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberGoodParts = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberFunctionalParts = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.WaferId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.FabWaferId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.WaferFrameId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.WaferMaskId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.WaferDescriptionUser = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.WaferDescriptionExec = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            ushort length = 30;
            if (this.WaferId == null)
            {
                // No change to record length. Not going to even be in the record.
                // Also, short-circuits checking other properties.
                return length;
            }
            else if (this.WaferId.Length > 255)
            {
                throw new Stdf4ParserException("WRR.WaferId is too long! Max allowed is 255 characters.");
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
            ushort idx = 2; // SKipping the first two length bytes.

            destinationByteArray[offset + idx] = this.RecordType;
            idx += 1;

            destinationByteArray[offset + idx] = this.RecordSubtype;
            idx += 1;

            destinationByteArray[offset + idx] = this.TestHeadNumber;
            idx += 1;

            destinationByteArray[offset + idx] = this.SiteGroupNumber;
            idx += 1;

            idx += this._valueConverter.SetUint32(this.LastPartTestedTs, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberPartsTested, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberPartsRetested, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberAbortsDuringTesting, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberGoodParts, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberFunctionalParts, destinationByteArray, offset + idx);

            bool havePrevoiusNull = false; // See the summary of WriteAsciiString for more info.
            idx += this._valueConverter.WriteAsciiString(this.WaferId, destinationByteArray, havePreviousNull: ref havePrevoiusNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.FabWaferId, destinationByteArray, havePreviousNull: ref havePrevoiusNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.WaferFrameId, destinationByteArray, havePreviousNull: ref havePrevoiusNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.WaferMaskId, destinationByteArray, havePreviousNull: ref havePrevoiusNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.WaferDescriptionUser, destinationByteArray, havePreviousNull: ref havePrevoiusNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.WaferDescriptionExec, destinationByteArray, havePreviousNull: ref havePrevoiusNull, offset: idx + offset);

            // Lastly set the length, which is the first field so no + idx with the offset nor to accumulate length.
            this._valueConverter.SetUint16(idx, destinationByteArray, offset: offset);
            return idx;
        }

    }
}
