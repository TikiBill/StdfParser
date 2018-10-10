// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class MRR : Stdf4Record
    {
        // For DI, so we can convert between big and little-endian.
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "MRR";
        public override byte RecordType { get; } = 1;
        public override byte RecordSubtype { get; } = 20;

        public uint LastPartTestedTs { get; set; }
        public char? LotDispositionCode { get; set; } = null; //Sigh, it can be missing.
        public string LotDescriptionUser { get; set; }
        public string LotDescriptionExec { get; set; }

        public MRR() { }

        public MRR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public MRR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   Disposition {this.LotDispositionCode}    UserDescription {this.LotDescriptionUser}    ExecDescription {this.LotDescriptionExec}";

        public MRR Parse(byte[] recordData)
        {
            return this.Parse(recordData.AsSpan(), offset: 0);
        }

        public MRR Parse(ReadOnlySpan<byte> recordData, int offset = 0)
        {
            this.LastPartTestedTs = this._valueConverter.GetUInt32AndUpdateOffset(recordData, ref offset);
            this.LotDispositionCode = this._valueConverter.GetNullableCharAndUpdateOffset(recordData, ref offset);
            this.LotDescriptionUser = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            this.LotDescriptionExec = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);

            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            ushort length = 8; //Header plus time-stamp

            length += (ushort)(this.LotDispositionCode == null ? 0 : 1);

            // The extra byte is for the length field for the string.
            length += (ushort)(this.LotDescriptionUser == null ? 0 : this.LotDescriptionUser.Length + 1);
            length += (ushort)(this.LotDescriptionExec == null ? 0 : this.LotDescriptionExec.Length + 1);

            return length;
        }


        /// <inheritdoc/>
        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {
            ushort idx = 2; // SKipping the first two length bytes.

            idx += this._valueConverter.SetByte(this.RecordType, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetByte(this.RecordSubtype, destinationByteArray, offset: idx + offset);
            idx += this._valueConverter.SetUint32(this.LastPartTestedTs, destinationByteArray, offset: idx + offset);

            bool havePrevoiusNull = false; // See the summary of WriteAsciiString for more info.
            idx += this._valueConverter.WriteNullableChar(this.LotDispositionCode, destinationByteArray, offset: idx + offset, havePreviousNull: ref havePrevoiusNull);
            idx += this._valueConverter.WriteAsciiString(this.LotDescriptionUser, destinationByteArray, offset: idx + offset, havePreviousNull: ref havePrevoiusNull);
            idx += this._valueConverter.WriteAsciiString(this.LotDescriptionExec, destinationByteArray, offset: idx + offset, havePreviousNull: ref havePrevoiusNull);

            // Lastly set the length, which is the first field so no + idx with the offset nor to accumulate length.
            this._valueConverter.SetUint16(idx, destinationByteArray, offset: offset);
            return idx;
        }

    }
}
