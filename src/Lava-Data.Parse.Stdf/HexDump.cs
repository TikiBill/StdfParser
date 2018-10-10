// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using System.Text;

namespace LavaData.Util.Debug
{
    public class HexDump
    {

        private byte[] _bytes;

        public int BytesPerLine { get; set; } = 16;

        public bool ShowAscii { get; set; } = true;

        public bool ShowOffset { get; set; } = true;


        public HexDump(byte[] bytes)
        {
            this._bytes = bytes;
        }

        public override string ToString()
        {
            if (_bytes == null)
            {
                return "<null>";
            }

            var sb = new StringBuilder();
            int lastStart = 0;
            for (int index = 0; index < this._bytes.Length; index++)
            {
                if (index % this.BytesPerLine == 0)
                {
                    if (index > 0)
                    {
                        if (this.ShowAscii)
                        {
                            sb.Append("  ");
                            sb.Append(this.ToFilteredAscii(lastStart, index - lastStart));
                        }
                        sb.AppendLine();
                    }

                    if (this.ShowOffset)
                    {
                        sb.Append(this.FormatOffset(index));
                        sb.Append("  ");
                    }
                    lastStart = index;
                }

                sb.Append($"{this._bytes[index]:X2} ");
            }

            var padding = this.BytesPerLine - (this._bytes.Length % this.BytesPerLine);
            if (this.ShowAscii && padding > 0)
            {
                for (int i = 0; i < padding; i++)
                {
                    sb.Append(".. ");
                }
                sb.Append("  ");
                sb.Append(this.ToFilteredAscii(lastStart, this._bytes.Length - lastStart));
            }
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// Return a string of bytes suitable for putting in
        /// c# code, e.g. for testing.
        /// </summary>
        public string ToHexByteString()
        {
            if (_bytes == null)
            {
                return "0x00";
            }
            var bytes = new System.Collections.Generic.List<string>();
            var sb = new StringBuilder();
            for (int index = 0; index < this._bytes.Length; index++)
            {
                if (index > 0)
                {
                    sb.Append(", ");
                }

                if (index % 8 == 0)
                {
                    sb.Append("\n");
                }

                sb.Append($" 0x{this._bytes[index]:X2}");
            }

            return sb.ToString();
        }

        private string FormatOffset(int offset) => $"{offset:X8}";

        private string ToFilteredAscii(int start, int len)
        {
            var byteSpan = this._bytes.AsSpan().Slice(start, len).ToArray();
            for (int i = 0; i < byteSpan.Length; i++)
            {
                if (byteSpan[i] < 32)
                {
                    byteSpan[i] = 0x2E;
                }
            }
            return Encoding.ASCII.GetString(byteSpan);
        }

    }
}
