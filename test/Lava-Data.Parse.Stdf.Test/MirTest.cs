// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using LavaData.Parse.Stdf4.Records;
using Xunit;

namespace Lava_Data.Parse.Stdf.Test
{
    public class MirTest
    {
        // Little-endian MIR record
        public static byte[] Mir01 = new byte[]{
            0x00,  0x01,  0x01,  0x0A,  0x46,  0x08,  0xBE,  0x5B,
            0x5A,  0x0D,  0xBE,  0x5B,  0x01,  0x50,  0x58,  0x43,
            0x0A,  0x00,  0x51,  0x06,  0x4C,  0x6F,  0x74,  0x31,
            0x32,  0x33,  0x0A,  0x50,  0x72,  0x6F,  0x64,  0x75,
            0x63,  0x74,  0x32,  0x33,  0x34,  0x07,  0x4E,  0x6F,
            0x64,  0x65,  0x31,  0x32,  0x34,  0x04,  0x54,  0x54,
            0x33,  0x32,  0x04,  0x4A,  0x6F,  0x62,  0x32,  0x05,
            0x52,  0x65,  0x76,  0x2E,  0x35,  0x07,  0x53,  0x75,
            0x62,  0x6C,  0x6F,  0x74,  0x37,  0x03,  0x4F,  0x70,
            0x34,  0x04,  0x45,  0x54,  0x34,  0x33,  0x06,  0x45,
            0x54,  0x52,  0x65,  0x76,  0x32,  0x04,  0x54,  0x43,
            0x39,  0x47,  0x03,  0x31,  0x32,  0x43,  0x09,  0x55,
            0x73,  0x65,  0x72,  0x54,  0x65,  0x78,  0x74,  0x31,
            0x0A,  0x41,  0x75,  0x78,  0x46,  0x69,  0x6C,  0x65,
            0x5A,  0x79,  0x7A,  0x0B,  0x50,  0x61,  0x63,  0x6B,
            0x61,  0x67,  0x65,  0x20,  0x52,  0x65,  0x64,  0x08,
            0x46,  0x61,  0x6D,  0x69,  0x6C,  0x79,  0x20,  0x51,
            0x08,  0x44,  0x61,  0x74,  0x65,  0x43,  0x20,  0x33,
            0x34,  0x05,  0x46,  0x61,  0x63,  0x32,  0x34,  0x07,
            0x46,  0x6C,  0x6F,  0x6F,  0x72,  0x20,  0x52,  0x08,
            0x50,  0x72,  0x6F,  0x63,  0x20,  0x45,  0x52,  0x54,
            0x08,  0x48,  0x69,  0x67,  0x68,  0x46,  0x72,  0x65,
            0x71,  0x06,  0x53,  0x70,  0x65,  0x63,  0x20,  0x47,
            0x0A,  0x53,  0x70,  0x65,  0x63,  0x20,  0x52,  0x65,
            0x76,  0x20,  0x43,  0x06,  0x46,  0x6C,  0x6F,  0x77,
            0x20,  0x54,  0x07,  0x53,  0x65,  0x74,  0x75,  0x70,
            0x20,  0x4D,  0x0C,  0x44,  0x65,  0x73,  0x69,  0x67,
            0x6E,  0x20,  0x52,  0x65,  0x76,  0x20,  0x50,  0x08,
            0x45,  0x6E,  0x67,  0x4C,  0x6F,  0x74,  0x20,  0x36,
            0x07,  0x52,  0x6F,  0x6D,  0x20,  0x52,  0x45,  0x4D,
            0x06,  0x53,  0x65,  0x72,  0x20,  0x39,  0x30,  0x0C,
            0x53,  0x75,  0x70,  0x65,  0x72,  0x20,  0x50,  0x65,
            0x72,  0x73,  0x6F,  0x6E
        };


        [Fact]
        public void TestParseMir01_Offset()
        {
            var converter = new StdfValueConverter();
            converter.CpuType = 2;
            var referenceMir = Mir01_Source(converter);
            var mir = new MIR(Mir01, converter: converter, offset: 4);
            Assert.Equal(referenceMir.SetupTimeStamp, mir.SetupTimeStamp);
            Assert.Equal(referenceMir.StartTimeStamp, mir.StartTimeStamp);
            Assert.Equal(referenceMir.StationNumber, mir.StationNumber);
            Assert.Equal(referenceMir.TestModeCode, mir.TestModeCode);
            Assert.Equal(referenceMir.LotRetestCode, mir.LotRetestCode);
            Assert.Equal(referenceMir.DataProtectionCode, mir.DataProtectionCode);
            Assert.Equal(referenceMir.BurnInTimeMinutes, mir.BurnInTimeMinutes);
            Assert.Equal(referenceMir.CommandModeCode, mir.CommandModeCode);
            Assert.Equal(referenceMir.LotId, mir.LotId);
            Assert.Equal(referenceMir.ProductId, mir.ProductId);
            Assert.Equal(referenceMir.NodeName, mir.NodeName);
            Assert.Equal(referenceMir.TesterType, mir.TesterType);
            Assert.Equal(referenceMir.JobName, mir.JobName);
            Assert.Equal(referenceMir.JobRevision, mir.JobRevision);
            Assert.Equal(referenceMir.SublotId, mir.SublotId);
            Assert.Equal(referenceMir.OperatorId, mir.OperatorId);
            Assert.Equal(referenceMir.ExecutiveType, mir.ExecutiveType);
            Assert.Equal(referenceMir.ExecutiveVersion, mir.ExecutiveVersion);
            Assert.Equal(referenceMir.TestCode, mir.TestCode);
            Assert.Equal(referenceMir.TestTemperature, mir.TestTemperature);
            Assert.Equal(referenceMir.UserText, mir.UserText);
            Assert.Equal(referenceMir.AuxFile, mir.AuxFile);
            Assert.Equal(referenceMir.PackageType, mir.PackageType);
            Assert.Equal(referenceMir.ProductFamilyId, mir.ProductFamilyId);
            Assert.Equal(referenceMir.DateCode, mir.DateCode);
            Assert.Equal(referenceMir.TestFacilityId, mir.TestFacilityId);
            Assert.Equal(referenceMir.TestFloorId, mir.TestFloorId);
            Assert.Equal(referenceMir.FabProcessId, mir.FabProcessId);
            Assert.Equal(referenceMir.OperationFrequency, mir.OperationFrequency);
            Assert.Equal(referenceMir.TestSpecName, mir.TestSpecName);
            Assert.Equal(referenceMir.TestSpecVersion, mir.TestSpecVersion);
            Assert.Equal(referenceMir.TestFlowId, mir.TestFlowId);
            Assert.Equal(referenceMir.TestSetupId, mir.TestSetupId);
            Assert.Equal(referenceMir.DeviceDesignRev, mir.DeviceDesignRev);
            Assert.Equal(referenceMir.EngineringLotId, mir.EngineringLotId);
            Assert.Equal(referenceMir.RomCodeId, mir.RomCodeId);
            Assert.Equal(referenceMir.TesterSerialNumber, mir.TesterSerialNumber);
            Assert.Equal(referenceMir.SupervisorId, mir.SupervisorId);
        }
        [Fact]
        public void TestParseMir01_Span()
        {
            var converter = new StdfValueConverter();
            converter.CpuType = 2;
            var referenceMir = Mir01_Source(converter);
            var mir = new MIR(Mir01.AsSpan().Slice(4), converter: converter);
            Assert.Equal(referenceMir.SetupTimeStamp, mir.SetupTimeStamp);
            Assert.Equal(referenceMir.StartTimeStamp, mir.StartTimeStamp);
            Assert.Equal(referenceMir.StationNumber, mir.StationNumber);
            Assert.Equal(referenceMir.TestModeCode, mir.TestModeCode);
            Assert.Equal(referenceMir.LotRetestCode, mir.LotRetestCode);
            Assert.Equal(referenceMir.DataProtectionCode, mir.DataProtectionCode);
            Assert.Equal(referenceMir.BurnInTimeMinutes, mir.BurnInTimeMinutes);
            Assert.Equal(referenceMir.CommandModeCode, mir.CommandModeCode);
            Assert.Equal(referenceMir.LotId, mir.LotId);
            Assert.Equal(referenceMir.ProductId, mir.ProductId);
            Assert.Equal(referenceMir.NodeName, mir.NodeName);
            Assert.Equal(referenceMir.TesterType, mir.TesterType);
            Assert.Equal(referenceMir.JobName, mir.JobName);
            Assert.Equal(referenceMir.JobRevision, mir.JobRevision);
            Assert.Equal(referenceMir.SublotId, mir.SublotId);
            Assert.Equal(referenceMir.OperatorId, mir.OperatorId);
            Assert.Equal(referenceMir.ExecutiveType, mir.ExecutiveType);
            Assert.Equal(referenceMir.ExecutiveVersion, mir.ExecutiveVersion);
            Assert.Equal(referenceMir.TestCode, mir.TestCode);
            Assert.Equal(referenceMir.TestTemperature, mir.TestTemperature);
            Assert.Equal(referenceMir.UserText, mir.UserText);
            Assert.Equal(referenceMir.AuxFile, mir.AuxFile);
            Assert.Equal(referenceMir.PackageType, mir.PackageType);
            Assert.Equal(referenceMir.ProductFamilyId, mir.ProductFamilyId);
            Assert.Equal(referenceMir.DateCode, mir.DateCode);
            Assert.Equal(referenceMir.TestFacilityId, mir.TestFacilityId);
            Assert.Equal(referenceMir.TestFloorId, mir.TestFloorId);
            Assert.Equal(referenceMir.FabProcessId, mir.FabProcessId);
            Assert.Equal(referenceMir.OperationFrequency, mir.OperationFrequency);
            Assert.Equal(referenceMir.TestSpecName, mir.TestSpecName);
            Assert.Equal(referenceMir.TestSpecVersion, mir.TestSpecVersion);
            Assert.Equal(referenceMir.TestFlowId, mir.TestFlowId);
            Assert.Equal(referenceMir.TestSetupId, mir.TestSetupId);
            Assert.Equal(referenceMir.DeviceDesignRev, mir.DeviceDesignRev);
            Assert.Equal(referenceMir.EngineringLotId, mir.EngineringLotId);
            Assert.Equal(referenceMir.RomCodeId, mir.RomCodeId);
            Assert.Equal(referenceMir.TesterSerialNumber, mir.TesterSerialNumber);
            Assert.Equal(referenceMir.SupervisorId, mir.SupervisorId);
        }


        [Fact]
        public void TestWriteMir01()
        {
            var converter = new StdfValueConverter();
            converter.CpuType = 2;
            var mir = Mir01_Source(converter);

            // For debuging and creating the byte-array to paste into this unit test file.
            // Console.Write((new LavaData.Util.Debug.HexDump(mir.ToBytes())).ToString());
            // Console.WriteLine(string.Empty);
            // Console.Write((new LavaData.Util.Debug.HexDump(mir.ToBytes())).ToHexByteString());
            // Console.WriteLine(string.Empty);

            Assert.Equal(Mir01, mir.ToBytes());
        }


        /// <summary>
        /// New MIR record with unique values in each field.
        /// 
        /// This has to be created in a method because tests
        /// can run in parallel and we don't want the same converter
        /// stepping on its self across tests.
        /// </summary>
        public MIR Mir01_Source(StdfValueConverter converter)
        {
            return new MIR(converter)
            {
                SetupTimeStamp = 1539180614,
                StartTimeStamp = 1539181914,
                StationNumber = 1,
                TestModeCode = 'P',
                LotRetestCode = 'X',
                DataProtectionCode = 'C',
                BurnInTimeMinutes = 10,
                CommandModeCode = 'Q',
                LotId = "Lot123",
                ProductId = "Product234",
                NodeName = "Node124",
                TesterType = "TT32",
                JobName = "Job2",
                JobRevision = "Rev.5",
                SublotId = "Sublot7",
                OperatorId = "Op4",
                ExecutiveType = "ET43",
                ExecutiveVersion = "ETRev2",
                TestCode = "TC9G",
                TestTemperature = "12C",
                UserText = "UserText1",
                AuxFile = "AuxFileZyz",
                PackageType = "Package Red",
                ProductFamilyId = "Family Q",
                DateCode = "DateC 34",
                TestFacilityId = "Fac24",
                TestFloorId = "Floor R",
                FabProcessId = "Proc ERT",
                OperationFrequency = "HighFreq",
                TestSpecName = "Spec G",
                TestSpecVersion = "Spec Rev C",
                TestFlowId = "Flow T",
                TestSetupId = "Setup M",
                DeviceDesignRev = "Design Rev P",
                EngineringLotId = "EngLot 6",
                RomCodeId = "Rom REM",
                TesterSerialNumber = "Ser 90",
                SupervisorId = "Super Person",
            };
        }
    }
}
