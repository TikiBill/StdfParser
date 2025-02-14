// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
#pragma warning disable IDE0130 // folder structure
#pragma warning disable S101 // pascal naming convention.
#pragma warning disable S2325 // static method.
// spell-checker:ignore stdf

namespace LavaData.Parse.Stdf4.Records
{
    /// <summary>
    /// Methods to convert values between STDF records, the file's
    /// endianness, and .NET types.
    /// 
    /// A single thread should inject the same instance into the constructor
    /// of each record type. This is so the byte arrays used in the conversion
    /// process will be reused, thereby reducing GC pressure.
    /// </summary>
    public class StdfValueConverter
    {
        // Public byte arrays for use in reversing bytes.
        // Part of the class so that we can reuse the byte
        // array (reducing GC pressure) while also keeping
        // parsing thread safe by not sharing memory.
        // These are public so that, e.g. PTR decoding can
        // use them inline.
        public byte[] TwoBytes { get; } = new byte[2];
        public byte[] FourBytes { get; } = new byte[4];
        public byte[] TwoFiftyFiveBytes { get; } = new byte[255];

        private byte _cpuType = BitConverter.IsLittleEndian ? (byte)2 : (byte)1;

        /// <summary>
        /// The CPU type, 1 = big endian, 2 = little endian.
        /// 
        /// NOTE: SETTING THIS HAS A SIDE EFFECT!
        /// 
        /// When setting the CPU type, the ReverseBytesOnRead and
        /// ReverseBytesOnWrite properties will be updated to match
        /// if byte reversal is needed during the read/write operation.
        /// In normal cases, this is fine and automatic determination
        /// is a desired effect. However, there is the possibility that
        /// someone will want to read in one byte order and write in
        /// another and the two flags allow for that situation.
        /// </summary>
        public byte CpuType
        {
            get
            {
                return this._cpuType;
            }

            set
            {
                this._cpuType = value;
                if (this._cpuType == 0)
                {
                    throw new Stdf4ParserException("Conversion to/from DEC PDP-11/VAX not supported");
                }
                else if ((this._cpuType == 2 && BitConverter.IsLittleEndian) || (this._cpuType == 1 && !BitConverter.IsLittleEndian))
                {
                    this.ReverseBytesOnRead = false;
                    this.ReverseBytesOnWrite = false;
                }
                else if (this._cpuType == 1 || this._cpuType == 2)
                {
                    this.ReverseBytesOnRead = true;
                    this.ReverseBytesOnWrite = true;
                }
                else
                {
                    throw new Stdf4ParserException("Unsupported CPU type! Only values of 1 or 2 are allowed.");
                }
            }
        }

        /// <summary>
        /// Reverse the bytes on read, i.e. when changing endianness during a read.
        /// 
        /// NOTE: Setting the CpuType changes this value!
        /// 
        /// Normally this and ReverseBytesOnWrite will be the same value
        /// but you can change them if you need to do an endianness conversion.
        /// </summary>
        public bool ReverseBytesOnRead { get; set; } = false;

        /// <summary>
        /// Reverse the bytes on write, i.e. when changing endianness during a write.
        /// 
        /// NOTE: Setting the CpuType changes this value!
        /// 
        /// Normally this and ReverseBytesOnRead will be the same value
        /// but you can change them if you need to do an endianness conversion.
        /// </summary>
        public bool ReverseBytesOnWrite { get; set; } = false;

        /// <summary>
        /// Get two bytes from the passed data. Considers this.ReverseBytesOnRead
        /// and reverses the byte order if true.
        /// </summary>
        public byte[] GetTwoBytes(ReadOnlySpan<byte> data, int offset = 0)
        {
            if (offset + 1 >= data.Length)
            {
                throw new Stdf4ParserException($"Data length is {data.Length} but trying to get a value from {offset+1}");
            }

            if (this.ReverseBytesOnRead)
            {
                (TwoBytes[0], TwoBytes[1]) = (data[offset + 1], data[offset + 0]);
            }
            else
            {
                (TwoBytes[0], TwoBytes[1]) = (data[offset + 0], data[offset + 1]);
            }
            return TwoBytes;
        }

        /// <summary>
        /// Get two bytes from the passed data. Considers this.ReverseBytesOnRead
        /// and reverses the byte order if true.
        /// </summary>
        public byte[] GetTwoBytes(byte[] data, int offset = 0)
        {
            if (this.ReverseBytesOnRead)
            {
                (TwoBytes[0], TwoBytes[1]) = (data[offset + 1], data[offset + 0]);
            }
            else
            {
                (TwoBytes[0], TwoBytes[1]) = (data[offset + 0], data[offset + 1]);
            }
            return TwoBytes;
        }


        public short GetInt16AndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            short value = BitConverter.ToInt16(this.GetTwoBytes(data, offset), 0);
            offset += 2;
            return value;
        }

        public ushort GetUInt16AndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            ushort value = BitConverter.ToUInt16(this.GetTwoBytes(data, offset), 0);
            offset += 2;
            return value;
        }


        /// <summary>
        /// Get four bytes from the passed data. Considers this.ReverseBytesOnRead
        /// and reverses the byte order if true.
        /// </summary>
        public byte[] GetFourBytes(ReadOnlySpan<byte> data, int offset = 0)
        {
            if (this.ReverseBytesOnRead)
            {
                (this.FourBytes[0], this.FourBytes[1], this.FourBytes[2], this.FourBytes[3])
                 = (data[offset + 3], data[offset + 2], data[offset + 1], data[offset + 0]);
            }
            else
            {
                (this.FourBytes[0], this.FourBytes[1], this.FourBytes[2], this.FourBytes[3])
                 = (data[offset + 0], data[offset + 1], data[offset + 2], data[offset + 3]);
            }
            return FourBytes;
        }

        /// <summary>
        /// Get four bytes from the passed data. Considers this.ReverseBytesOnRead
        /// and reverses the byte order if true.
        /// </summary>
        public byte[] GetFourBytes(byte[] data, int offset = 0)
        {
            if (this.ReverseBytesOnRead)
            {
                (this.FourBytes[0], this.FourBytes[1], this.FourBytes[2], this.FourBytes[3])
                 = (data[offset + 3], data[offset + 2], data[offset + 1], data[offset + 0]);
            }
            else
            {
                (this.FourBytes[0], this.FourBytes[1], this.FourBytes[2], this.FourBytes[3])
                 = (data[offset + 0], data[offset + 1], data[offset + 2], data[offset + 3]);
            }
            return FourBytes;
        }


        public int GetInt32AndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            int value = BitConverter.ToInt32(this.GetFourBytes(data, offset), 0);
            offset += 4;
            return value;
        }

        public uint GetUInt32AndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset + 4 > data.Length)
            {
                return 0;
            }

            uint value = BitConverter.ToUInt32(this.GetFourBytes(data, offset), 0);
            offset += 4;
            return value;
        }

        public (float, bool) GetSingleAndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset + 4 > data.Length)
            {
                // Could throw an exception here.
                return (0.0f, false);
            }

            float value = BitConverter.ToSingle(this.GetFourBytes(data, offset), 0);
            offset += 4;
            return (value, true);
        }


        public ushort SetByte(byte value, byte[] intoData, int offset = 0)
        {
            intoData[offset] = value;
            return 1;
        }

        public ushort SetAsciiChar(char value, byte[] intoData, int offset = 0)
        {
            if (value > 0xff)
            {
                // Technically 7f is the upper limit of non-extended ASCII, but we will not overflow at 255.
                throw new Stdf4ParserException("Character value out of range. Needs to be in the ASCII range.");
            }
            intoData[offset] = (byte)((int)value & 0xff);
            return 1;
        }

        /// <summary>
        /// Set two bytes in a byte array, reversing byte order as indicated
        /// by this.ReverseBytesOnWrite
        /// </summary>
        public ushort SetUint16(ushort value, byte[] intoData, int offset = 0)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                intoData[offset + 0] = bytes[1];
                intoData[offset + 1] = bytes[0];
            }
            else
            {
                intoData[offset + 0] = bytes[0];
                intoData[offset + 1] = bytes[1];
            }
            return 2;
        }

        /// <summary>
        /// Set two bytes in a byte array, reversing byte order as indicated
        /// by this.ReverseBytesOnWrite
        /// 
        /// This is the int version since uint16 math gets cast to an int. As
        /// such this can throw an ArgumentOutOfRangeException if the value is
        /// too large or negative.
        /// </summary>
        public ushort SetUint16(int value, byte[] intoData, int offset = 0)
        {
            if (value > UInt16.MaxValue || value < 0)
            {
                throw new ArgumentOutOfRangeException("Cannot convert to a UInt16 -- value out of range.");
            }

            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                intoData[offset + 0] = bytes[1];
                intoData[offset + 1] = bytes[0];
            }
            else
            {
                intoData[offset + 0] = bytes[0];
                intoData[offset + 1] = bytes[1];
            }
            return 2;
        }


        /// <summary>
        /// Set four bytes in a byte array, reversing byte order as indicated
        /// by this.ReverseBytesOnWrite
        /// </summary>
        public ushort SetUint32(uint value, byte[] intoData, int offset = 0)
        {
            // Using shift does not work without special checks and
            // manipulation because it does not account for endianness.
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                intoData[offset + 0] = bytes[3];
                intoData[offset + 1] = bytes[2];
                intoData[offset + 2] = bytes[1];
                intoData[offset + 3] = bytes[0];
            }
            else
            {
                intoData[offset + 0] = bytes[0];
                intoData[offset + 1] = bytes[1];
                intoData[offset + 2] = bytes[2];
                intoData[offset + 3] = bytes[3];
            }
            return 4;
        }


        /// <summary>
        /// Get a string from an STDF record, and update the passed offset.
        /// 
        /// If the offset is greater than or equal to the length of the data,
        /// then null is returned. If the length is zero, then the empty string
        /// is returned.
        /// 
        /// Returning a null is because there are files with records in which
        /// the record just ends if there is no further data to report.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset">Index in the data, updated after the string read.</param>
        /// <returns>The string.</returns>
        public string? GetStringAndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset >= data.Length)
            {
                return null;
            }

            int len = data[offset];
            if (len == 0)
            {
                offset += 1;
                return string.Empty;
            }

            if (offset + 1 + len > data.Length)
            {
                throw new Stdf4ParserException("Invalid Data: Could not get a string, not enough data left in the record."
                 + " (Len is " + len + " and we have " + (data.Length - offset - 1) + " bytes left.)");
            }

            string str = System.Text.Encoding.ASCII.GetString(data.Slice(offset + 1, len).ToArray());
            offset += len + 1;
            return str;
        }


        public char? GetNullableCharAndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset >= data.Length)
            {
                return null;
            }

            string str = System.Text.Encoding.ASCII.GetString(data.Slice(offset, 1).ToArray());
            offset += 1;
            return str[0];
        }

        /// <summary>
        /// Write out an ASCII string to the given byte array. Does not include
        /// a training null since the STDF record type does not call for it.
        /// 
        /// A value of null will write zero bytes and set the havePreviousNull
        /// flag to true, the empty string will write a single length byte of zero.
        /// 
        /// The havePreviousNull ref parameter is to track if pervious property had
        /// a null value. STDF records cannot handle a null then non-null field (only zero
        /// length) so if someone creates a record without setting the in-between fields
        /// to the empty string or an invalid value (for nullable types) it should
        /// raise an exception.
        /// </summary>
        /// <param name="value">The string to write out.</param>
        /// <param name="intoData">The byte array to alter.</param>
        /// <param name="havePreviousNull">ref to flag if we have seen a null property.</param>
        /// <param name="offset">Offset into the byte array. Optional, default 0.</param>
        /// <returns>The number of bytes written.</returns>
        public ushort WriteAsciiString(string? value, byte[] intoData, ref bool havePreviousNull, int offset = 0)
        {
            if (value is null)
            {
                havePreviousNull = true;
                return 0; //Nothing written.
            }
            else if (havePreviousNull)
            {
                throw new Stdf4ParserException("When writing a record, there is a non-null property after a null property.");
            }
            else if (value == string.Empty)
            {
                intoData[offset + 0] = 0;
                return 1;
            }
            else if (value.Length > 255)
            {
                throw new Stdf4ParserException("String value is too long, max is 255 characters in an STDF field.");
            }
            else
            {
                intoData[offset + 0] = (byte)value.Length;
                var asciiString = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                for (int i = 0; i < asciiString.Length; i++)
                {
                    intoData[offset + i + 1] = asciiString[i];
                }
                return (ushort)(value.Length + 1);
            }
        }

        public ushort WriteNullableChar(char? value, byte[] intoData, ref bool havePreviousNull, int offset = 0)
        {
            if (value == null)
            {
                havePreviousNull = true;
                return 0; //Nothing written.
            }
            else if (havePreviousNull)
            {
                throw new Stdf4ParserException("When writing a record, there is a non-null property after a null property.");
            }
            else if ((int)value > 0xff)
            {
                throw new Stdf4ParserException("When writing a record, a char is out of the extended ASCII range.");
            }
            else
            {
                intoData[offset] = (byte)((int)value & 0xff);
                return 1;
            }
        }


        public (byte, byte) UshortToBytes(ushort value)
        {
            // Using shift does not work without special checks and
            // manipulation because it does not account for endianness.
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                return (bytes[1], bytes[0]);
            }
            else
            {
                return (bytes[0], bytes[1]);
            }
        }

        /// <summary>
        /// Return a tuple of bytes based on the given value.
        /// 
        /// Set reverseBytes to true if you are changing the endian-ness
        /// of the value.
        /// </summary>
        public (byte, byte, byte, byte) UintToBytes(uint value, bool reverseBytes)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                return (bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            else
            {
                return (bytes[0], bytes[1], bytes[2], bytes[3]);
            }
        }

        public ushort WriteAsciiString(string value, byte[] intoData, int offset = 0)
        {
            if (value == null)
            {
                intoData[offset + 0] = 0;
                return 1;
            }
            else if (value.Length > 255)
            {
                throw new Stdf4ParserException("String value is too long, max is 255 characters in an STDF field.");
            }
            else
            {
                var asciiString = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                for (int i = 0; i < asciiString.Length; i++)
                {
                    intoData[offset + i] = asciiString[i];
                }
                return (ushort)value.Length;
            }
        }

    }
}
