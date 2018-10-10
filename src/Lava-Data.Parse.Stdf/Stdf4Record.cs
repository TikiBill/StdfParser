// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;

namespace LavaData.Parse.Stdf4.Records
{
    /// <summary>
    /// Abstract class for STDF records. Every record type should inherit
    /// from this class.
    /// </summary>
    public abstract class Stdf4Record
    {
        public abstract string RecordName { get; }
        public abstract byte RecordType { get; }
        public abstract byte RecordSubtype { get; }

        /// <summary>
        /// The length of the record in bytes when ToBytes() is called, or how
        /// much space SetBytes() will write. This includes the record length
        /// (UInt16) and the field type + field sub-type, which will always
        /// consume 4 bytes.
        /// 
        /// The returned value is a ushort (U*2) so that the value can be used
        /// in building up the record (by subtracting 4). If you are using this to determine
        /// the entire length, be sure to accumulate in an int, uint or bigger.
        /// 
        /// Also, generally there are no data validity checks, such as variable length
        /// fields being 255 bytes or less, only ASCII characters, or an illegal
        /// null/not-null sequence.
        /// </summary>
        public abstract ushort Length();

        public ushort LengthNoHeader() => (ushort)(this.Length() - 4);

        /// <summary>
        /// Return a new byte array of the record including the record
        /// type and sub-type.
        /// 
        /// Creates a byte array based on this.Length()
        /// and then calls this.SetBytes().
        /// </summary>
        public byte[] ToBytes()
        {

            var data = new byte[this.Length()];
            this.SetBytes(data, offset: 0);
            return data;
        }

        /// <summary>
        /// Set the bytes in the passed byte array to represent the record.
        /// Returns the number of bytes set.
        /// 
        /// Presumably you can call Length() on all records, pre-allocate
        /// a byte array, and then call SetBytes() for each record, moving
        /// the offset forward by the number returned.
        /// </summary>
        /// <param name="destinationByteArray">The byte array which will be altered.</param>
        /// <param name="offset">The offset at which to write.</param>
        /// <returns>The number of bytes written.</returns>
        public abstract ushort SetBytes(byte[] destinationByteArray, int offset = 0);

    }
}
