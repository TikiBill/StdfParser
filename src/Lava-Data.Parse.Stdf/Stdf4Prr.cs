// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class PRR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "PRR";
        public override byte RecordType { get; } = 5;
        public override byte RecordSubtype { get; } = 20;

        public byte TestHeadNumber { get; set; }
        public byte TestSiteNumber { get; set; }

        public byte PartInformationFlag { get; set; }

        public bool DataSupersedesPreviousPart
        {
            get => (this.PartInformationFlag & 0b0000_0001) == 0 ? false : true;
            set
            {
                if (value) { this.PartInformationFlag |= 0b0000_0001; } else { this.PartInformationFlag &= 0b1111_1110; }
            }
        }

        public bool TestingCompletedNormally
        {
            get => (this.PartInformationFlag & 0b0000_0001) == 0 ? true : false;
            set
            {
                if (value) { this.PartInformationFlag &= 0b1111_1110; } else { this.PartInformationFlag |= 0b0000_0001; }
            }
        }

        public bool PartPassed
        {
            get => (this.PartInformationFlag & 0b0000_0001) == 0 ? true : false;
            set
            {
                if (value) { this.PartInformationFlag &= 0b1111_1110; } else { this.PartInformationFlag |= 0b0000_0001; }
            }
        }

        public bool PartFailFlagValid
        {
            get => (this.PartInformationFlag & 0b0000_0001) == 0 ? false : true;
            set
            {
                if (value) { this.PartInformationFlag |= 0b0000_0001; } else { this.PartInformationFlag &= 0b1111_1110; }
            }
        }

        public ushort NumberTestsExecuted { get; set; }
        public ushort HardwareBinNumber { get; set; }
        public ushort SoftwareBinNumber { get; set; }
        public short CoordinateX { get; set; }
        public short CoordinateY { get; set; }
        public uint ElapsedTimeMs { get; set; }
        public string PartIdentification { get; set; }
        public string PartDescription { get; set; }
        public byte[] PartRepairInformation { get; set; }

        public PRR() { }

        public PRR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public PRR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   ({this.CoordinateX}, {this.CoordinateY})   ID {this.PartIdentification}    HWBin {this.HardwareBinNumber}    SWBin {this.SoftwareBinNumber}";

        public PRR Parse(byte[] recordData, byte cpuType = 2)
        {
            return this.Parse(recordData.AsSpan(), offset: 0, cpuType: cpuType);
        }

        public PRR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            this.TestHeadNumber = recordData[offset];
            offset += 1;

            this.TestSiteNumber = recordData[offset];
            offset += 1;

            this.PartInformationFlag = recordData[offset];
            offset += 1;

            this.NumberTestsExecuted = this._valueConverter.GetUInt16AndUpdateOffset(recordData, ref offset);
            this.HardwareBinNumber = this._valueConverter.GetUInt16AndUpdateOffset(recordData, ref offset);
            this.SoftwareBinNumber = this._valueConverter.GetUInt16AndUpdateOffset(recordData, ref offset);
            this.CoordinateX = this._valueConverter.GetInt16AndUpdateOffset(recordData, ref offset);
            this.CoordinateY = this._valueConverter.GetInt16AndUpdateOffset(recordData, ref offset);
            this.ElapsedTimeMs = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.PartIdentification = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.PartDescription = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);

            if (offset >= recordData.Length) { return this; }

            // FIXME put this copy into a method like the GetString...
            int len = recordData[offset];
            this.PartRepairInformation = new byte[len];
            for (int i = 0; i < len; i++)
            {
                this.PartRepairInformation[i] = recordData[offset + i];
            }

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
            throw new Stdf4ParserException("Not Implemented");
            //return idx;
        }

    }
}
