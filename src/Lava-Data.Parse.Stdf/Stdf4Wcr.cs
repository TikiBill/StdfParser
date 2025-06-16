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
    public class WCR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private readonly StdfValueConverter _valueConverter;

        public override Stdf4RecordType Stdf4RecordType { get; } = Stdf4RecordType.WCR;
        public override string RecordName { get; } = "WCR";
        public override byte RecordType { get; } = 2;
        public override byte RecordSubtype { get; } = 30;

        public float WaferDiameter { get; set; }
        public float DieHeight { get; set; }
        public float DieWidth { get; set; }
        public byte UnitsCode { get; set; }
        public string Units
        {
            get
            {
                switch (this.UnitsCode)
                {
                    case 0:
                        return "Unknown";
                    case 1:
                        return "Inches";
                    case 2:
                        return "Centimeters";
                    case 3:
                        return "Millimeters";
                    case 4:
                        return "Mils";
                    default:
                        return "Invalid Code";
                }
            }
        }
        public char FlatLocation { get; set; }
        public short CenterDieX { get; set; }
        public short CenterDieY { get; set; }
        public char PosDirectionX { get; set; }
        public char PosDirectionY { get; set; }

        public WCR(StdfValueConverter converter)
        {
            this._valueConverter = converter;
        }

        public WCR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public WCR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   WaferDiameter: {this.WaferDiameter}    Die Size: {this.DieWidth} x {this.DieHeight}";

        public WCR Parse(byte[] recordData, int offset = 0, byte cpuType = 2)
        {
            return this.Parse(recordData.AsSpan(), offset: offset, cpuType: cpuType);
        }

        public WCR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            (this.WaferDiameter, _) = this._valueConverter.GetSingleAndUpdateOffset(recordData, ref offset);
            (this.DieHeight, _) = this._valueConverter.GetSingleAndUpdateOffset(recordData, ref offset);
            (this.DieWidth, _) = this._valueConverter.GetSingleAndUpdateOffset(recordData, ref offset);

            this.UnitsCode = recordData[offset];
            offset += 1;

            this.FlatLocation = (char)recordData[offset];
            offset += 1;

            if (offset >= recordData.Length)
            {
                return this;
            }

            this.CenterDieX = this._valueConverter.GetInt16AndUpdateOffset(recordData, ref offset);
            this.CenterDieY = this._valueConverter.GetInt16AndUpdateOffset(recordData, ref offset);

            this.PosDirectionX = (char)recordData[offset];
            offset += 1;

            this.PosDirectionY = (char)recordData[offset];

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            throw new Stdf4ParserException("Not Implemented");
            // ushort length = 10;
            // return length;
        }


        /// <inheritdoc/>
        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {
            throw new Stdf4ParserException("Not Implemented");
        }

    }
}
