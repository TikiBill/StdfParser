// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class PTR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "PTR";
        public override byte RecordType { get; } = 15;
        public override byte RecordSubtype { get; } = 10;

        public uint TestNumber { get; set; }
        public byte TestHeadNumber { get; set; }
        public byte TestSiteNumber { get; set; }

        public byte TestFlags { get; set; }

        public byte ParametricTestFlags { get; set; }
        public float TestResult { get; set; }
        public string TestDescription { get; set; }
        public string AlarmName { get; set; }
        public byte OptionalDataFlag { get; set; }
        public sbyte TestResultsScalingExponent { get; set; } //OptionalDataFlag bit 0 == 1
        public sbyte LowLimitScalingExponent { get; set; } //OptionalDataFlag bit 4 or 6 == 1
        public sbyte HighLimitScalingExponent { get; set; } //OptionalDataFlag bit 5 or 7 == 1
        public float LowTestLimit { get; set; } //OptionalDataFlag bit 4 or 6 == 1
        public float HighTestLimit { get; set; } //OptionalDataFlag bit 5 or 7 == 1
        public string TestUnits { get; set; }
        public string FormatStringResult { get; set; }
        public string FormatStringLowLimit { get; set; }
        public string FormatStringHighLimit { get; set; }
        public float LowSpecLimit { get; set; } //OptionalDataFlag bit 2 == 1
        public float HighSpecLimit { get; set; } //OptionalDataFlag bit 3 == 1

        public PTR() { }

        public PTR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public PTR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   {this.TestDescription}    {this.TestResult}";

        public PTR Parse(byte[] recordData)
        {
            return this.Parse(recordData.AsSpan(), offset: 0);
        }

        public PTR Parse(ReadOnlySpan<byte> recordData, int offset = 0)
        {
            int startOffset = offset;
            bool valid;
            byte[] b = this._valueConverter.FourBytes;

            // This is a very commonly called routine, so optimize it with
            // slightly less branching and many less calls to methods.
            //
            // Sample file went from ~590ms to ~380ms, with this and moving the PTR
            // check outside the enum cast in the parser.
            if (this._valueConverter.ReverseBytesOnRead)
            {
                (b[0], b[1], b[2], b[3]) = (recordData[3], recordData[2], recordData[1], recordData[0]);
                this.TestNumber = BitConverter.ToUInt32(b, 0);

                // Single addition makes it a little faster.
                this.TestHeadNumber = recordData[offset + 4];
                this.TestSiteNumber = recordData[offset + 5];
                this.TestFlags = recordData[offset + 6];
                this.ParametricTestFlags = recordData[offset + 7];

                (b[0], b[1], b[2], b[3]) = (recordData[11], recordData[10], recordData[9], recordData[8]);
                this.TestResult = BitConverter.ToSingle(b, 0);
            }
            else
            {
                (b[0], b[1], b[2], b[3]) = (recordData[0], recordData[1], recordData[2], recordData[3]);
                this.TestNumber = BitConverter.ToUInt32(b, 0);

                // Single addition makes it a little faster.
                this.TestHeadNumber = recordData[offset + 4];
                this.TestSiteNumber = recordData[offset + 5];
                this.TestFlags = recordData[offset + 6];
                this.ParametricTestFlags = recordData[offset + 7];

                (b[0], b[1], b[2], b[3]) = (recordData[8], recordData[9], recordData[10], recordData[11]);
                this.TestResult = BitConverter.ToSingle(b, 0);
            }

            offset += 12;
            if (offset >= recordData.Length) { return this; }

            int len = recordData[offset];

            // baa -- tried a byte-wise copy into a shared buffer, but it is slower by 50 ms.
            // Even copying four bytes in each loop is not faster.

            // if (true)
            // {
            //     var stringBuffer = this._valueConverter.TwoFiftyFiveBytes;
            //     int i = 0;
            //     for (; i < len - 3; i += 4)
            //     {
            //         stringBuffer[i] = recordData[i];
            //         stringBuffer[i + 1] = recordData[i + 1];
            //         stringBuffer[i + 2] = recordData[i + 2];
            //         stringBuffer[i + 3] = recordData[i + 3];
            //     }

            //     for (; i < len; i++)
            //     {
            //         stringBuffer[i] = recordData[i];
            //     }

            //     this.TestDescription = System.Text.Encoding.ASCII.GetString(stringBuffer, 0, len);
            // }
            // else
            // {
            this.TestDescription = System.Text.Encoding.ASCII.GetString(recordData.Slice(offset + 1, len).ToArray());
            // }
            offset += len + 1;

            // This is a good place to short-circuit the parsing since often the rest of the record is not present.
            // Note: a D10 often writes this field for every record. TODO: Some string caching.
            if (offset >= recordData.Length) { return this; }

            this.AlarmName = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            if (offset >= recordData.Length) { return this; }


            this.OptionalDataFlag = recordData[offset];
            offset += 1;

            this.TestResultsScalingExponent = (sbyte)recordData[offset];
            offset += 1;

            this.LowLimitScalingExponent = (sbyte)recordData[offset];
            offset += 1;

            this.HighLimitScalingExponent = (sbyte)recordData[offset];
            offset += 1;

            (this.LowTestLimit, valid) = this._valueConverter.GetSingleAndUpdateOffset(recordData, ref offset);
            (this.HighTestLimit, valid) = this._valueConverter.GetSingleAndUpdateOffset(recordData, ref offset);
            this.TestUnits = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.FormatStringResult = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.FormatStringLowLimit = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.FormatStringHighLimit = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            (this.LowSpecLimit, valid) = this._valueConverter.GetSingleAndUpdateOffset(recordData, ref offset);
            (this.HighSpecLimit, valid) = this._valueConverter.GetSingleAndUpdateOffset(recordData, ref offset);

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            throw new Stdf4ParserException("Not Implemented");
        }


        /// <inheritdoc/>
        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {
            ushort idx = 2; // SKipping the first two length bytes.

            destinationByteArray[offset] = this.RecordType;
            offset += 1;

            destinationByteArray[offset] = this.RecordSubtype;
            offset += 1;

            destinationByteArray[offset] = this.TestHeadNumber;
            offset += 1;

            destinationByteArray[offset] = this.TestSiteNumber;
            offset += 1;

            // Lastly set the length.
            (destinationByteArray[offset + 0], destinationByteArray[offset + 1]) = this._valueConverter.UshortToBytes(idx);

            throw new Stdf4ParserException("Not Implemented");
            //return idx;
        }

    }
}
