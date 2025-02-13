// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class SDR : Stdf4Record
    {

        // For DI, so we can convert between big and little-endian.
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "SDR";
        public override byte RecordType { get; } = 1;
        public override byte RecordSubtype { get; } = 80;

        public byte TestHeadNumber { get; set; }
        public byte SiteGroupNumber { get; set; }
        public byte NumberTestSitesInGroup { get; set; }
        public byte[] SiteNumbers { get; set; }
        public string HandlerType { get; set; }
        public string HandlerId { get; set; }
        public string ProbeCardType { get; set; }
        public string ProbeCardId { get; set; }
        public string LoadBoardType { get; set; }
        public string LoadBoardId { get; set; }
        public string DibBoardType { get; set; }
        public string DibBoardId { get; set; }
        public string InterfaceCableType { get; set; }
        public string InterfaceCableId { get; set; }
        public string HandlerContactorType { get; set; }
        public string HandlerContactId { get; set; }
        public string LaserType { get; set; }
        public string LaserId { get; set; }
        public string ExtraEquipmentType { get; set; }
        public string ExtraEquipmentId { get; set; }

        public SDR() { }

        public SDR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public SDR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"SDR   Handler {this.HandlerType}/{this.HandlerId}";

        public SDR Parse(byte[] recordData, byte cpuType = 2)
        {
            return this.Parse(recordData.AsSpan(), offset: 0, cpuType: cpuType);
        }

        public SDR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            int idx = 0;
            this.TestHeadNumber = recordData[offset + idx];
            idx += 1;

            this.SiteGroupNumber = recordData[offset + idx];
            idx += 1;

            this.NumberTestSitesInGroup = recordData[offset + idx];
            idx += 1;

            if (this.NumberTestSitesInGroup > 0)
            {
                this.SiteNumbers = new byte[this.NumberTestSitesInGroup];
                for (int i = 0; i < this.NumberTestSitesInGroup; i++)
                {
                    this.SiteNumbers[i] = recordData[offset + idx + i];
                }
                idx += this.NumberTestSitesInGroup;
            }
            else
            {
                this.SiteNumbers = null;
            }

            this.HandlerType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.HandlerId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.ProbeCardType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.ProbeCardId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.LoadBoardType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.LoadBoardId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.DibBoardType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.DibBoardType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.InterfaceCableType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.InterfaceCableId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.HandlerContactorType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.HandlerContactId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.LaserType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.LaserId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.ExtraEquipmentType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.ExtraEquipmentId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            ushort length = 7;
            if (this.HandlerType == null)
            {
                // No change to record length. Not going to even be in the record.
                return length; //Also, no more fields to be examined.
            }
            else if (this.HandlerType.Length > 255)
            {
                throw new Stdf4ParserException("SDR.HandlerType is too long! Max allowed is 255 characters.");
            }
            else
            {
                // The extra byte is for the length of the string.
                length += (ushort)(this.HandlerType.Length + 1);
            }
            throw new Stdf4ParserException("Not Implemented");
            // return length;
        }


        /// <inheritdoc/>
        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {
            throw new Stdf4ParserException("Not Implemented");
        }

    }
}
