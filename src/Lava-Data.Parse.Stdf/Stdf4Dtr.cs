// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.Serialization;

// https://www.inheritdoc.io/

namespace LavaData.Parse.Stdf4.Records
{
    public class DTR : Stdf4Record
    {
        private StdfValueConverter _valueConverter;

        public override string RecordName { get; } = "DTR";
        public override byte RecordType { get; } = 50;
        public override byte RecordSubtype { get; } = 30;

        public string Text { get; set; }

        public DTR() { }

        public DTR(ReadOnlySpan<byte> recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public DTR(byte[] recordData, StdfValueConverter converter, int offset = 0)
        {
            this._valueConverter = converter;
            this.Parse(recordData, offset);
        }

        public override string ToString() => $"{this.RecordName}   Text: {this.Text}";

        public DTR Parse(byte[] recordData, byte cpuType = 2)
        {
            return this.Parse(recordData.AsSpan(), offset: 0, cpuType: cpuType);
        }

        public DTR Parse(ReadOnlySpan<byte> recordData, int offset = 0, byte cpuType = 2)
        {
            this.Text = this._valueConverter.GetStringAndUpdateOffset(recordData, ref offset);
            return this;
        }

        /// <inheritdoc/>
        public override ushort Length()
        {
            ushort length = 4;
            if (this.Text == null)
            {
                // No change to record length. Not going to even be in the record.
            }
            else if (this.Text.Length > 255)
            {
                throw new Stdf4ParserException("DTR.Text is too long! Max allowed is 255 characters.");
            }
            else
            {
                // The extra byte is for the length of the string.
                length += (ushort)(this.Text.Length + 1);
            }
            return length;
        }


        /// <inheritdoc />
        public override ushort SetBytes(byte[] destinationByteArray, int offset = 0)
        {
            ushort length = this.LengthNoHeader();
            this._valueConverter.SetUint16(length, destinationByteArray, offset);
            destinationByteArray[offset + 2] = this.RecordType;
            destinationByteArray[offset + 3] = this.RecordSubtype;

            if (this.Text != null)
            {
                // Note the call to Length will throw an exception if the wafer ID is too long.
                destinationByteArray[offset + 4] = (byte)(this.Text.Length);
                var asciiString = System.Text.ASCIIEncoding.ASCII.GetBytes(this.Text);
                for (int i = 0; i < asciiString.Length; i++)
                {
                    destinationByteArray[offset + 5 + i] = asciiString[i];
                }
            }

            length += 4; // Four more bytes on the header.
            return length;
        }

    }
}
