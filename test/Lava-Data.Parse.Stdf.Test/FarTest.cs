// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using LavaData.Parse.Stdf4.Records;
using Xunit;

// spell-checker:ignore xunit stdf
namespace Lava_Data.Parse.Stdf.Test
{
    public class FarTest
    {
        // Little-endian FAR record.
        public static byte[] Far01 = new byte[]{
            0x02,  0x00,  0x00,  0x0A,  0x02,  0x04
        };

        [Fact]
        public void TestParseFar01()
        {
            var converter = new StdfValueConverter();
            converter.CpuType = 2;
            var far = new FAR(Far01, converter: converter, offset: 4);
            Assert.Equal(2, far.CpuType);
            Assert.Equal(4, far.StdfVersion);

            var far01b = Far01.AsSpan().Slice(4);
            far = new FAR(far01b, converter: converter);
            Assert.Equal(2, far.CpuType);
            Assert.Equal(4, far.StdfVersion);
        }


        [Fact]
        public void TestWriteFar01()
        {
            var converter = new StdfValueConverter();
            converter.CpuType = 2;
            var far = new FAR(converter);
            Assert.Equal(Far01, far.ToBytes());
        }

    }
}
