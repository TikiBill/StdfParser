// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;

#pragma warning disable IDE0130 // folder structure
#pragma warning disable S101 // pascal naming convention.
// spell-checker:ignore stdf

namespace LavaData.Parse.Stdf4.Records
{
    public class MIR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private readonly StdfValueConverter _valueConverter;

        public override Stdf4RecordType Stdf4RecordType { get; } = Stdf4RecordType.MIR;
        public override string RecordName { get; } = "MIR";
        public override byte RecordType { get; } = 1;
        public override byte RecordSubtype { get; } = 10;

        public uint SetupTimeStamp { get; set; }
        public uint StartTimeStamp { get; set; }
        public byte StationNumber { get; set; }
        public char TestModeCode { get; set; }
        public char LotRetestCode { get; set; }
        public char DataProtectionCode { get; set; }
        public ushort BurnInTimeMinutes { get; set; }
        public char CommandModeCode { get; set; }
        public string? LotId { get; set; }
        public string? ProductId { get; set; }
        public string? NodeName { get; set; }
        public string? TesterType { get; set; }
        public string? JobName { get; set; }
        public string? JobRevision { get; set; }
        public string? SubLotId { get; set; }
        public string? OperatorId { get; set; }
        public string? ExecutiveType { get; set; }
        public string? ExecutiveVersion { get; set; }
        public string? TestCode { get; set; }
        public string? TestTemperature { get; set; }
        public string? UserText { get; set; }
        public string? AuxFile { get; set; }
        public string? PackageType { get; set; }
        public string? ProductFamilyId { get; set; }
        public string? DateCode { get; set; }
        public string? TestFacilityId { get; set; }
        public string? TestFloorId { get; set; }
        public string? FabProcessId { get; set; }
        public string? OperationFrequency { get; set; }
        public string? TestSpecName { get; set; }
        public string? TestSpecVersion { get; set; }
        public string? TestFlowId { get; set; }
        public string? TestSetupId { get; set; }
        public string? DeviceDesignRev { get; set; }
        public string? EngineeringLotId { get; set; }
        public string? RomCodeId { get; set; }
        public string? TesterSerialNumber { get; set; }
        public string? SupervisorId { get; set; }

        public MIR(StdfValueConverter converter)
        {
            this._valueConverter = converter;
        }

        /// <summary>
        /// New MIR based on a bunch of bytes.
        /// </summary>
        /// <param name="recordData"></param>
        /// <param name="converter"></param>
        /// <param name="offset"></param>
        public MIR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset: offset);
        }

        /// <summary>
        /// New MIR based on a bunch of bytes.
        /// </summary>
        /// <param name="recordData"></param>
        /// <param name="converter"></param>
        /// <param name="offset"></param>
        public MIR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData.AsSpan(), offset: offset);
        }

        public MIR Parse(ReadOnlySpan<byte> recordData, int offset = 0)
        {

            this.SetupTimeStamp = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.StartTimeStamp = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);

            this.StationNumber = recordData[offset];
            offset += 1;

            this.TestModeCode = (char)recordData[offset];
            offset += 1;

            this.LotRetestCode = (char)recordData[offset];
            offset += 1;

            this.DataProtectionCode = (char)recordData[offset];
            offset += 1;

            this.BurnInTimeMinutes = this._valueConverter.GetUInt16AndUpdateOffset(recordData, ref offset);

            this.CommandModeCode = (char)recordData[offset];
            offset += 1;

            this.LotId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.ProductId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.NodeName = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TesterType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.JobName = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.JobRevision = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.SubLotId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.OperatorId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.ExecutiveType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.ExecutiveVersion = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestCode = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestTemperature = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.UserText = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.AuxFile = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.PackageType = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.ProductFamilyId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.DateCode = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestFacilityId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestFloorId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.FabProcessId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.OperationFrequency = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestSpecName = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestSpecVersion = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestFlowId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TestSetupId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.DeviceDesignRev = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.EngineeringLotId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.RomCodeId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.TesterSerialNumber = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.SupervisorId = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);

            return this;
        }


        public override string ToString()
        {
            return $"MIR  StartTS: {this.StartTimeStamp}  Station: {this.StationNumber}  TestMode: {this.TestModeCode}"
            + $"  LotId: {this.LotId}  ProductId: {this.ProductId}";
        }


        /// <inheritdoc/>
        public override ushort Length()
        {
            int len = 19;

            // The extra byte is for the length field for the string.
            len += this.LotId == null ? 0 : this.LotId.Length + 1;
            len += this.ProductId == null ? 0 : this.ProductId.Length + 1;
            len += this.NodeName == null ? 0 : this.NodeName.Length + 1;
            len += this.TesterType == null ? 0 : this.TesterType.Length + 1;
            len += this.JobName == null ? 0 : this.JobName.Length + 1;
            len += this.JobRevision == null ? 0 : this.JobRevision.Length + 1;
            len += this.SubLotId == null ? 0 : this.SubLotId.Length + 1;
            len += this.OperatorId == null ? 0 : this.OperatorId.Length + 1;
            len += this.ExecutiveType == null ? 0 : this.ExecutiveType.Length + 1;
            len += this.ExecutiveVersion == null ? 0 : this.ExecutiveVersion.Length + 1;
            len += this.TestCode == null ? 0 : this.TestCode.Length + 1;
            len += this.TestTemperature == null ? 0 : this.TestTemperature.Length + 1;
            len += this.UserText == null ? 0 : this.UserText.Length + 1;
            len += this.AuxFile == null ? 0 : this.AuxFile.Length + 1;
            len += this.PackageType == null ? 0 : this.PackageType.Length + 1;
            len += this.ProductFamilyId == null ? 0 : this.ProductFamilyId.Length + 1;
            len += this.DateCode == null ? 0 : this.DateCode.Length + 1;
            len += this.TestFacilityId == null ? 0 : this.TestFacilityId.Length + 1;
            len += this.TestFloorId == null ? 0 : this.TestFloorId.Length + 1;
            len += this.FabProcessId == null ? 0 : this.FabProcessId.Length + 1;
            len += this.OperationFrequency == null ? 0 : this.OperationFrequency.Length + 1;
            len += this.TestSpecName == null ? 0 : this.TestSpecName.Length + 1;
            len += this.TestSpecVersion == null ? 0 : this.TestSpecVersion.Length + 1;
            len += this.TestFlowId == null ? 0 : this.TestFlowId.Length + 1;
            len += this.TestSetupId == null ? 0 : this.TestSetupId.Length + 1;
            len += this.DeviceDesignRev == null ? 0 : this.DeviceDesignRev.Length + 1;
            len += this.EngineeringLotId == null ? 0 : this.EngineeringLotId.Length + 1;
            len += this.RomCodeId == null ? 0 : this.RomCodeId.Length + 1;
            len += this.TesterSerialNumber == null ? 0 : this.TesterSerialNumber.Length + 1;
            len += this.SupervisorId == null ? 0 : this.SupervisorId.Length + 1;

            if (len > UInt16.MaxValue)
            {
                throw new Stdf4ParserException("The record length is too long!");
            }

            return (ushort)len;
        }


        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {
            int idx = 2; //Skip the length bytes, we write them last after we know the overall length.
            idx += this._valueConverter.SetByte(this.RecordType, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetByte(this.RecordSubtype, destinationByteArray, offset: idx + offset);

            idx += this._valueConverter.SetUint32(this.SetupTimeStamp, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetUint32(this.StartTimeStamp, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetByte(this.StationNumber, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetAsciiChar(this.TestModeCode, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetAsciiChar(this.LotRetestCode, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetAsciiChar(this.DataProtectionCode, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetUint16(this.BurnInTimeMinutes, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetAsciiChar(this.CommandModeCode, destinationByteArray, offset: idx + offset);

            bool havePreviousNull = false; // See the summary of WriteAsciiString for more info.
            idx += this._valueConverter.WriteAsciiString(this.LotId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.ProductId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.NodeName, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TesterType, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.JobName, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.JobRevision, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.SubLotId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.OperatorId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.ExecutiveType, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.ExecutiveVersion, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TestCode, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TestTemperature, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.UserText, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.AuxFile, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.PackageType, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.ProductFamilyId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.DateCode, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TestFacilityId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TestFloorId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.FabProcessId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.OperationFrequency, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TestSpecName, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TestSpecVersion, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TestFlowId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TestSetupId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.DeviceDesignRev, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.EngineeringLotId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.RomCodeId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.TesterSerialNumber, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this._valueConverter.WriteAsciiString(this.SupervisorId, destinationByteArray, havePreviousNull: ref havePreviousNull, offset: idx + offset);

            // Lastly set the length, which is the first field so no + idx with the offset nor to accumulate length.
            // Also note that the first four bytes do not count in the record length value.
            this._valueConverter.SetUint16(idx - 4, destinationByteArray, offset: offset);
            return (ushort)idx;
        }
    }
}
