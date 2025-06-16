// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using LavaData.Parse.Stdf4.Records;
using Microsoft.Extensions.Logging;

#pragma warning disable S125 // remove commented out code.
#pragma warning disable IDE0290 // Use primary constructor
#pragma warning disable IDE0130 // folder structure

// spell-checker:ignore stdf eose
namespace LavaData.Parse.Stdf4
{
    public class Stdf4Parser
    {
        private readonly ILogger _logger;

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
        private static int _initialListCapacity = 1_000_000;

        public static void SetInitialRecordCapacity(int newCapacity)
        {
            _initialListCapacity = newCapacity;
        }

        /// <summary>
        /// Some of the more esoteric record types are not handled (parsed).
        /// The counts of those are noted in this dictionary in case one wants
        /// some insight to such ignored records.
        /// </summary>
        public Dictionary<string, int> IgnoredRecordTypeCount { get; } = [];

        public List<Stdf4Record> Records { get; }

        // e.g. (int)Stdf4RecordType.WCR
        public int DebugRecordType { get; set; } = (int)Stdf4RecordType.WCR;

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

        public string InputFile { get; }
        public bool IsStateValid { get; private set; }
        public string? InvalidStateMessage { get; private set; }
        private readonly BinaryReader? _externalReader;

        public Stdf4Parser(ILoggerFactory loggerFactory, string inputFile, BinaryReader? reader = null)
        {
            this._logger = loggerFactory.CreateLogger(nameof(Stdf4Parser));
            this.InputFile = inputFile;
            this._externalReader = reader;
            this.Records = new(_initialListCapacity);
            this.IsStateValid = false;
            this.InvalidStateMessage = "Parse not called yet.";
        }

        public Stdf4Parser(string inputFile, BinaryReader? reader = null)
            : this(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance, inputFile, reader)
        {

        }

        public void NoteIgnoredRecord(string stdfRecordType)
        {
            if (!this.IgnoredRecordTypeCount.ContainsKey(stdfRecordType))
            {
                this.IgnoredRecordTypeCount.Add(stdfRecordType, 0);
            }
            this.IgnoredRecordTypeCount[stdfRecordType] += 1;
        }

        public bool TryParse()
        {
            if (this._externalReader is not null)
            {
                try
                {
                    return this.TryParse(this._externalReader);
                }
                catch (Exception ex)
                {
                    this.IsStateValid = false;
                    this.InvalidStateMessage = $"Got an exception parsing: {ex.Message}";
                    return false;
                }
            }
            else if (!File.Exists(this.InputFile))
            {
                this.IsStateValid = false;
                this.InvalidStateMessage = "The input file does not exist.";
                return false;
            }

            this._logger.LogInformation("Read {InputFile}", this.InputFile);
            try
            {
                using var reader = new BinaryReader(File.Open(this.InputFile, FileMode.Open));
                return this.TryParse(reader);
            }
            catch (Exception ex)
            {
                this.IsStateValid = false;
                this.InvalidStateMessage = $"Got an exception parsing: {ex.Message}";
                return false;
            }
        }


#pragma warning disable S3776 // Refactor this method to reduce its Cognitive Complexity from 55 to the 15 allowed.
        public bool TryParse(BinaryReader reader, Stdf4RecordType stopAfterFirstRecordType = Stdf4RecordType.NUL)
        {
            int pos = 0;
            int recordNumber = 0;
            Stdf4Record? rec;
            StdfValueConverter converter = new();

            this.IsStateValid = true; // Until we have an error.
            this.InvalidStateMessage = string.Empty;

            this.Records.Clear();

            try
            {
                // Do not use nullable here to prevent unboxing on the stop check below.
                Stdf4RecordType recordType = Stdf4RecordType.NUL;
                ushort recordLength;
                while (true)
                {
                    if (recordNumber > 0 && recordType == stopAfterFirstRecordType)
                    {
                        break;
                    }

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
                        recordType = Stdf4RecordType.PTR;
                        if (this.OnlyParse)
                        {
                            _ = new PTR(bytes, converter);
                        }
                        else
                        {
                            this.Records.Add(new PTR(bytes, converter));
                        }
                        continue;
                    }
                    else if (stdfRecordType == 1290)
                    {
                        recordType = Stdf4RecordType.PIR;
                        if (this.OnlyParse)
                        {
                            _ = new PIR(bytes, converter);
                        }
                        else
                        {
                            this.Records.Add(new PIR(bytes, converter));
                        }
                        continue;
                    }

                    if (stdfRecordType == this.DebugRecordType || this._debugLevel >= 3 || (this.Verbose && stdfMajorType != 5 && stdfMajorType != 10 && stdfMajorType != 15))
                    {
                        Console.WriteLine(string.Empty);
                        Console.WriteLine($"  Record {recordNumber} is {recordLength} bytes long (0x{recordLength:X2}) of type {stdfMajorType} - {stdfMinorType} ({stdfRecordType}).");
                        if (this._debugLevel > 0 || stdfRecordType == this.DebugRecordType)
                        {
                            Console.Write((new LavaData.Util.Debug.HexDump(bytes)).ToString());
                            if (this._debugLevel > 1 || stdfRecordType == this.DebugRecordType)
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
                        recordType = (Stdf4RecordType)stdfRecordType;
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

                            case Stdf4RecordType.ATR:
                                this.NoteIgnoredRecord("ATR");
                                break;

                            case Stdf4RecordType.EPS:
                                this.NoteIgnoredRecord("EPS");
                                break;

                            case Stdf4RecordType.BPS:
                                this.NoteIgnoredRecord("BPS");
                                break;

                            case Stdf4RecordType.FAR:
                                var far = new FAR(bytes, converter);
                                converter.CpuType = far.CpuType;
                                this.ReverseBytesOnRead = converter.ReverseBytesOnRead;
                                rec = far;
                                this._logger.LogDebug("FAR: {Far}", far.ToString()); // STDF Version: {Version}    CPU Type: {CpuType}", far.StdfVersion, far.CpuType);
                                break;

                            //Data collected on a per lot basis; 1-NN Records
                            case Stdf4RecordType.MIR:
                                rec = new MIR(bytes, converter);
                                this._logger.LogDebug("MIR: {Mir}", rec.ToString());
                                break;

                            case Stdf4RecordType.MRR:
                                rec = new MRR(bytes, converter);
                                this._logger.LogDebug("MRR: {Mrr}", rec.ToString());
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
                                this.NoteIgnoredRecord("PMR");
                                // throw new Stdf4ParserException("PMR  Not Implemented");
                                break;

                            case Stdf4RecordType.PGR:
                                this.NoteIgnoredRecord("PGR");
                                // throw new Stdf4ParserException("PGR  Not Implemented");
                                break;

                            case Stdf4RecordType.RDR:
                                this.NoteIgnoredRecord("RDR");
                                // throw new Stdf4ParserException("RDR  Not Implemented");
                                break;

                            case Stdf4RecordType.SDR:
                                rec = new SDR(bytes, converter);
                                break;

                            // Data collected per Wafer; 2-NN Records.
                            case Stdf4RecordType.WIR:
                                rec = new WIR(bytes, converter);
                                this._logger.LogDebug("WIR: {Wir}", rec.ToString());
                                break;

                            case Stdf4RecordType.WRR:
                                rec = new WRR(bytes, converter);
                                this._logger.LogDebug("WRR: {Wrr}", rec.ToString());
                                break;

                            case Stdf4RecordType.WCR:
                                rec = new WCR(bytes, converter);
                                this._logger.LogDebug("WCR: {Wcr}", rec.ToString());
                                break;

                            case Stdf4RecordType.TSR:
                                rec = new TSR(bytes, converter);
                                break;

                            // Generic Data; 50-NN Records.
                            case Stdf4RecordType.GDR:
                                this.NoteIgnoredRecord("GDR");
                                //throw new Stdf4ParserException("GDR  Not Implemented");
                                break;

                            case Stdf4RecordType.DTR:
                                rec = new DTR(bytes, converter);
                                break;

                            default:
                                this.IsStateValid = false;
                                this.InvalidStateMessage = $"Unhandled (no case statement) STDF4 Record Type: {recordType} ({stdfMajorType} - {stdfMinorType})";
                                this._logger.LogError("{Message}", this.InvalidStateMessage);
                                break;
                        }

                        if (rec is not null)
                        {
                            if (this._debugLevel >= 3 || (this.Verbose && stdfRecordType != 1300))
                            {
                                // Don't print PRR (1300), lots of them, except at a high debug level.
                                this._logger.LogInformation("{RecName}: {StringValue}", rec.RecordName, rec.ToString());
                            }

                            if (!this.OnlyParse)
                            {
                                this.Records.Add(rec);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  No ENUM Defined for STDF4 Record Type: {stdfMajorType} - {stdfMinorType}.");
                    }
                }
            }
            catch (EndOfStreamException eose)
            {
#pragma warning disable S6667 // Pass exception to logging.
                this._logger.LogTrace("Read to the end of the stream: {ExceptionMessage}", eose.Message);
#pragma warning restore S6667
            }
            catch (Exception ex)
            {
                this.IsStateValid = false;
                this.InvalidStateMessage = $"Unhandled exception: {ex.Message}";
                this._logger.LogError(ex, "{Message}", this.InvalidStateMessage);
            }

            this._logger.LogDebug("Read {Count} records.", recordNumber);

            return this.IsStateValid;
        }
#pragma warning restore S3776 
    }
}
