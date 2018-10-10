// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using LavaData.Parse.Stdf4.Records;

namespace LavaData.Parse.Stdf4
{
    public class Stdf4Parser
    {
        // Remember, readonly here means we cannot assign to a different
        // two bytes, but we can change the values within.
        private readonly byte[] _twoBytes = new byte[2];

        public bool ReverseBytesOnRead { get; set; } = false;

        /// <summary>
        /// Set to true and the parsed data will not be saved,
        /// this saving a little time and allocation. Mostly
        /// for benchmarking the parsing.
        /// </summary>
        public bool OnlyParse { get; set; } = false;

        /// <summary>
        /// The initial capacity of the list used to store the
        /// records. Set this to a reasonable value a little larger
        /// than the common file you'll be parsing to avoid
        /// the increase capacity/copy cycle.
        /// </summary>
        public int InitialListCapacity { get; set; } = 1_000_000;

        public bool Verbose { get; set; } = false;

        private int _debugLevel = 0;

        public int DebugLevel
        {
            get
            {
                return this._debugLevel;
            }
            set
            {
                this._debugLevel = value;
                if (value > 0)
                {
                    this.Verbose = true;
                }
            }
        }

        public List<Stdf4Record> ReadStdf4(string inputFile)
        {
            if (File.Exists(inputFile))
            {
                if (this.Verbose)
                {
                    Console.WriteLine($"Opening {inputFile}");
                }

                using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
                {
                    return this.ReadStdf4(reader);
                }
            }
            else
            {
                Console.Error.WriteLine($"Error: {inputFile} does not exist!");
            }

            return null;
        }


        public List<Stdf4Record> ReadStdf4(BinaryReader reader)
        {
            List<Stdf4Record> records;
            if (this.OnlyParse)
            {
                records = null;
            }
            else
            {
                records = new List<Stdf4Record>(capacity: this.InitialListCapacity);
            }

            int pos = 0;
            int recordNumber = 0;
            Stdf4Record rec;
            StdfValueConverter converter = new StdfValueConverter();
            try
            {
                ushort recordLength;
                while (true)
                {
                    if (this.ReverseBytesOnRead)
                    {
                        this._twoBytes[1] = reader.ReadByte();
                        this._twoBytes[0] = reader.ReadByte();
                        recordLength = BitConverter.ToUInt16(_twoBytes, 0);
                    }
                    else
                    {
                        recordLength = reader.ReadUInt16();
                    }

                    var stdfMajorType = reader.ReadByte();
                    var stdfMinorType = reader.ReadByte();
                    int stdfRecordType = (((int)stdfMajorType) << 8) | (int)stdfMinorType;

                    // for a FAR record, always read 6 bytes since we don't know the byte order yet.
                    // Also avoiding a cast here.
                    byte[] bytes;
                    if (stdfRecordType == 10)
                    {
                        recordLength = 2;
                    }

                    bytes = reader.ReadBytes(recordLength);
                    pos += recordLength + 4;
                    recordNumber += 1;

                    //Three most common record types without a cast and outside the switch statement
                    if (this._debugLevel >= 3)
                    {
                        // Fall through to case statement for lots of printing.
                    }
                    else if (stdfRecordType == 3850)
                    {
                        if (this.OnlyParse)
                        {
                            var ignore_PTR = new PTR(bytes, converter);
                        }
                        else
                        {
                            records.Add(new PTR(bytes, converter));
                        }
                        continue;
                    }
                    else if (stdfRecordType == 1290)
                    {
                        if (this.OnlyParse)
                        {
                            var ignore_PIR = new PIR(bytes, converter);
                        }
                        else
                        {
                            records.Add(new PIR(bytes, converter));
                        }
                        continue;
                    }

                    if (this._debugLevel >= 3 || (this.Verbose && stdfMajorType != 5 && stdfMajorType != 10 && stdfMajorType != 15))
                    {
                        Console.WriteLine(string.Empty);
                        Console.WriteLine($"  Record {recordNumber} is {recordLength} bytes long (0x{recordLength:X2}) of type {stdfMajorType} - {stdfMinorType} ({stdfRecordType}).");
                        if (this._debugLevel > 0)
                        {
                            Console.Write((new LavaData.Util.Debug.HexDump(bytes)).ToString());
                            if (this._debugLevel > 1)
                            {
                                // We want the record length and record type/sub-type.
                                var newBytes = new byte[bytes.Length + 4];
                                (newBytes[0], newBytes[1]) = converter.UshortToBytes(recordLength);
                                newBytes[2] = stdfMajorType;
                                newBytes[3] = stdfMinorType;
                                for (int i = 0; i < bytes.Length; i++) { newBytes[i + 4] = bytes[i]; }
                                Console.WriteLine((new LavaData.Util.Debug.HexDump(newBytes).ToHexByteString()));
                            }
                        }
                    }

                    rec = null;
                    if (Enum.IsDefined(typeof(Stdf4RecordType), stdfRecordType))
                    {
                        Stdf4RecordType recordType = (Stdf4RecordType)stdfRecordType;
                        switch ((Stdf4RecordType)stdfRecordType)
                        {
                            case Stdf4RecordType.PTR:
                                // UNREACHABLE CODE -- handled above, except with debug >=3
                                rec = new PTR(bytes, converter);
                                break;

                            case Stdf4RecordType.PIR:
                                // UNREACHABLE CODE -- handled above, except with debug >=3
                                rec = new PIR(bytes, converter);
                                break;

                            case Stdf4RecordType.PRR:
                                rec = new PRR(bytes, converter);
                                break;

                            case Stdf4RecordType.FAR:
                                var far = new FAR(bytes, converter);
                                converter.CpuType = far.CpuType;
                                this.ReverseBytesOnRead = converter.ReverseBytesOnRead;
                                rec = far;
                                break;

                            //Data collected on a per lot basis; 1-NN Records
                            case Stdf4RecordType.MIR:
                                rec = new MIR(bytes, converter);
                                break;

                            case Stdf4RecordType.MRR:
                                rec = new MRR(bytes, converter);
                                break;

                            case Stdf4RecordType.PCR:
                                rec = new PCR(bytes, converter);
                                break;

                            case Stdf4RecordType.HBR:
                                rec = new HBR(bytes, converter);
                                break;

                            case Stdf4RecordType.SBR:
                                rec = new SBR(bytes, converter);
                                break;

                            case Stdf4RecordType.PMR:
                                throw new Stdf4ParserException("PMR  Not Implemented");

                            case Stdf4RecordType.PGR:
                                throw new Stdf4ParserException("PGR  Not Implemented");

                            case Stdf4RecordType.RDR:
                                throw new Stdf4ParserException("RDR  Not Implemented");

                            case Stdf4RecordType.SDR:
                                rec = new SDR(bytes, converter);
                                break;

                            // Data collected per Wafer; 2-NN Records.
                            case Stdf4RecordType.WIR:
                                rec = new WIR(bytes, converter);
                                break;

                            case Stdf4RecordType.WRR:
                                rec = new WRR(bytes, converter);
                                break;

                            case Stdf4RecordType.WCR:
                                rec = new WCR(bytes, converter);
                                break;

                            case Stdf4RecordType.TSR:
                                //throw new Stdf4ParserException("TSR  Not Implemented");
                                break;


                            // Generic Data; 50-NN Records.
                            case Stdf4RecordType.GDR:
                                //throw new Stdf4ParserException("GDR  Not Implemented");
                                break;

                            case Stdf4RecordType.DTR:
                                rec = new DTR(bytes, converter);
                                break;

                            default:
                                Console.WriteLine($"  Unhandled STDF4 Record Type: {recordType} ({stdfMajorType} - {stdfMinorType}).");
                                break;
                        }

                        if (rec != null)
                        {
                            if (this._debugLevel >= 3 || (this.Verbose && stdfRecordType != 1300))
                            {
                                // Don't print PRR (1300), lots of them, except at a high debug level.
                                Console.WriteLine(rec.ToString());
                            }

                            if (!this.OnlyParse)
                            {
                                records.Add(rec);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  No ENUM Defined for STDF4 Record Type: {stdfMajorType} - {stdfMinorType}.");
                    }
                }
            }
            catch (EndOfStreamException)
            {
                if (this.Verbose)
                {
                    Console.WriteLine("Read to the end of the stream!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled Exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            if (this.Verbose)
            {
                Console.WriteLine($"Read {recordNumber} records.");
            }

            return records;
        }
    }
}
