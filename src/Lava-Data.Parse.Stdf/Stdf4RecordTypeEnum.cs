// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

#pragma warning disable S2437 // Remove this unnecessary bit operation. baa: It is there to be explicit.
namespace LavaData.Parse.Stdf4
{
    public enum Stdf4RecordType
    {
        /// <summary>
        /// Internal type to prevent unboxing when we are stopping at a specific record type.
        /// </summary>
        NUL = 0,

        /// <summary>
        /// File Attributes Record (FAR) 0 - 10
        /// </summary>
        FAR = (0 << 8) | 10,
        /// <summary>
        /// Audit Trail Record (ATR) 0 - 20
        /// </summary>
        ATR = (0 << 8) | 20,

        /// <summary>
        /// Master Information Record (MIR) 1- 10
        /// </summary>
        MIR = (1 << 8) | 10,
        /// <summary>
        /// Master Results Record (MRR) 1 - 20
        /// </summary>
        MRR = (1 << 8) | 20,
        PCR = (1 << 8) | 30,
        HBR = (1 << 8) | 40,
        SBR = (1 << 8) | 50,
        /// <summary>
        /// Pin Map Record (PMR) 1 - 60
        /// </summary>
        PMR = (1 << 8) | 60,
        /// <summary>
        /// Pin Group Record (PGR) 1 - 62
        /// </summary>
        PGR = (1 << 8) | 62,
        PLR = (1 << 8) | 63,
        /// <summary>
        /// Retest Data Record (RDR) 1 - 70
        /// </summary>
        RDR = (1 << 8) | 70,
        SDR = (1 << 8) | 80,

        WIR = (2 << 8) | 10,
        WRR = (2 << 8) | 20,
        WCR = (2 << 8) | 30,

        /// <summary>
        /// Part Information Record (PIR) 5 - 10
        /// </summary>
        PIR = (5 << 8) | 10,
        /// <summary>
        /// Part Results Record (PRR) 5 - 20
        /// </summary>
        PRR = (5 << 8) | 20,

        /// <summary>
        /// Test Synopsis Record (TSR)
        /// </summary>
        TSR = (10 << 8) | 30,

        /// <summary>
        /// Parametric Test Record (PTR) 15 - 10
        /// </summary>
        PTR = (15 << 8) | 10,
        MPR = (15 << 8) | 15,
        FTR = (15 << 8) | 20,

        BPS = (20 << 8) | 10,
        EPS = (20 << 8) | 20,

        /// <summary>
        /// Generic Data Record (GDR) 50-10
        /// </summary>
        GDR = (50 << 8) | 10,
        DTR = (50 << 8) | 30,
    }
}
