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
    public class TSR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private readonly StdfValueConverter _valueConverter;

        public override Stdf4RecordType Stdf4RecordType { get; } = Stdf4RecordType.TSR;
        public override string RecordName { get; } = "TSR";
        public override byte RecordType { get; } = 10;
        public override byte RecordSubtype { get; } = 30;

        public byte TestHeadNumber { get; set; }
        public byte TestSiteNumber { get; set; }
        public char TestType { get; set; } = ' ';
        public uint TestNumber { get; set; }
        public uint NumberOfTestExecutions { get; set; }
        public uint NumberOfTestFailures { get; set; }
        public uint NumberOfAlarmedTests { get; set; }
        public string? TestName { get; set; } = string.Empty;
        public string? TestSequencerName { get; set; } = string.Empty;
        public string? TestLabel { get; set; } = string.Empty;
        public byte OptionalDataFlag { get; set; }
        public float? AverageTestTimeSeconds { get; set; }
        public float? MinTestResultValue { get; set; }
        public float? MaxTestResultValue { get; set; }
        public float? SumTestResultValues { get; set; }
        public float? SumSquareTestResultValues { get; set; }

        public TSR(StdfValueConverter converter)
        {
            this._valueConverter = converter;
        }

        public TSR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public TSR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   TestNumber: {this.TestNumber}    TestName: {this.TestName}    NumberOfTestExecutions: {this.NumberOfTestExecutions}";

        public TSR Parse(byte[] recordData, byte cpuType = 2)
        {
            return this.Parse(recordData.AsSpan(), offset: 0, cpuType: cpuType);
        }

        public TSR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            this.TestHeadNumber = recordData[offset];
            offset += 1;

            this.TestSiteNumber = recordData[offset];
            offset += 1;

            this.TestType = (char)recordData[offset];
            offset += 1;

            this.TestNumber = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberOfTestExecutions = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberOfTestFailures = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.NumberOfAlarmedTests = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);

            this.TestName = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestSequencerName = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestLabel = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);

            if (offset >= recordData.Length)
            {
                this.OptionalDataFlag = 0;
                return this;
            }

            this.OptionalDataFlag = recordData[offset];
            offset += 1;

            if ((this.OptionalDataFlag & 1 << 2) == 1)
            {
                this.AverageTestTimeSeconds = this._valueConverter.GetNullableSingleAndUpdateOffset(recordData, ref offset);
            }
            else
            {
                this.AverageTestTimeSeconds = null;
            }

            if ((this.OptionalDataFlag & 1) == 1)
            {
                this.MinTestResultValue = this._valueConverter.GetNullableSingleAndUpdateOffset(recordData, ref offset);
            }

            if ((this.OptionalDataFlag & 1 << 1) == 1)
            {
                this.MaxTestResultValue = this._valueConverter.GetNullableSingleAndUpdateOffset(recordData, ref offset);
            }

            if ((this.OptionalDataFlag & 1 << 4) == 1)
            {
                this.SumTestResultValues = this._valueConverter.GetNullableSingleAndUpdateOffset(recordData, ref offset);
            }

            if ((this.OptionalDataFlag & 1 << 5) == 1)
            {
                this.SumSquareTestResultValues = this._valueConverter.GetNullableSingleAndUpdateOffset(recordData, ref offset);
            }

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            ushort length = 40;
            if (this.TestName is null)
            {
                /* no length change */
            }
            else if (this.TestName.Length > 255)
            {
                throw new Stdf4ParserException("TSR.TestName is too long! Max allowed is 255 characters.");
            }
            else
            {
                // The extra byte is for the length of the string.
                length += (ushort)(this.TestName.Length + 1);
            }

            if (this.TestSequencerName is null)
            {
                /* no length change */
            }
            else if (this.TestSequencerName.Length > 255)
            {
                throw new Stdf4ParserException("TSR.TestSequencerName is too long! Max allowed is 255 characters.");
            }
            else
            {
                length += (ushort)(this.TestSequencerName.Length + 1);
            }

            if (this.TestLabel is null)
            {
                /* no length change */
            }
            else if (this.TestLabel.Length > 255)
            {
                throw new Stdf4ParserException("TSR.TestLabel is too long! Max allowed is 255 characters.");
            }
            else
            {
                length += (ushort)(this.TestLabel.Length + 1);
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

            destinationByteArray[offset + idx] = Convert.ToByte(this.TestType);
            idx += 1;

            idx += this._valueConverter.SetUint32(this.NumberOfTestExecutions, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberOfTestFailures, destinationByteArray, offset + idx);
            idx += this._valueConverter.SetUint32(this.NumberOfAlarmedTests, destinationByteArray, offset + idx);

            idx += this._valueConverter.WriteAsciiString(this.TestName, destinationByteArray, offset + idx);
            idx += this._valueConverter.WriteAsciiString(this.TestSequencerName, destinationByteArray, offset + idx);
            idx += this._valueConverter.WriteAsciiString(this.TestLabel, destinationByteArray, offset + idx);

            destinationByteArray[offset + idx] = this.OptionalDataFlag;
            idx += 1;

            // FIXME: Need to handle null on the optional data flag.            
            // idx += this._valueConverter.SetSingle(this.AverageTestTimeSeconds, destinationByteArray, offset + idx);
            // idx += this._valueConverter.SetSingle(this.MinTestResultValue, destinationByteArray, offset + idx);
            // idx += this._valueConverter.SetSingle(this.MaxTestResultValue, destinationByteArray, offset + idx);
            // idx += this._valueConverter.SetSingle(this.SumTestResultValues, destinationByteArray, offset + idx);
            // idx += this._valueConverter.SetSingle(this.SumSquareTestResultValues, destinationByteArray, offset + idx);

            // Lastly set the length.
            idx += this._valueConverter.SetUint16(idx, destinationByteArray, offset); // No + idx! Goes in first location.
            return idx;
        }

    }
}
