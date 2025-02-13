// Copyright (c) 2018 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using LavaData.Parse.Stdf4;
using CommandLine;
using System.IO;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

// spell-checker:ignore stdf
namespace LavaData.Parse.Stdf.ConsoleDumper
{
    class Options
    {
        [Option('i', "input-file", Required = false, HelpText = "The file to parse")]
        public string InputFile { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('t', "time", Required = false, HelpText = "Time parsing the file. Does not store results, useful for high-level (and questionably accurate) benchmark.")]
        public bool TimeParse { get; set; }

        [Option('d', "debug", HelpText = "Debug level. 1 = Dump rare records (MIR, MRR, etc), 3 = Dump ALL records.")]
        public int DebugLevel { get; set; }
    }

    class Program
    {

        static void Main(string[] args)
        {
            var loggerFactory = StaticProgramHelper.CreateLoggerFactory(2);
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Begin...");

            var stdf4Parser = new Stdf4Parser();

            var opts = Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (o.Verbose)
                       {
                           stdf4Parser.Verbose = o.Verbose;
                       }

                       stdf4Parser.DebugLevel = o.DebugLevel;

                       if (string.IsNullOrWhiteSpace(o.InputFile))
                       {
                           logger.LogError("Please provide an input file w/ -i FILE");
                       }
                       else if (!File.Exists(o.InputFile))
                       {
                           logger.LogError("ERROR: Your file {InputFile} does not exist.", o.InputFile);
                       }
                       else if (o.TimeParse)
                       {
                           if (o.Verbose)
                           {
                               logger.LogInformation("Parsing {InputFile}", o.InputFile);
                           }
                           stdf4Parser.OnlyParse = true;
                           Stopwatch stopwatch = new Stopwatch();

                           //First, time reading the file from disk.
                           stopwatch.Start();
                           using (BinaryReader reader = new BinaryReader(File.Open(o.InputFile, FileMode.Open)))
                           {
                               try
                               {
                                   while (true)
                                   {
                                       reader.ReadInt32();
                                   }
                               }
                               catch (EndOfStreamException)
                               {
                                   /* Ignore, we just wanted to read from the disk without a lot of checks. */
                               }
                           }
                           stopwatch.Stop();
                           logger.LogInformation("Time to read the file 32 bits at a time: {ElapsedTime}", stopwatch.Elapsed);

                           stopwatch.Reset();
                           stopwatch.Start();
                           stdf4Parser.ReadStdf4(o.InputFile);
                           stopwatch.Stop();
                           logger.LogInformation("         Time to Parse w/o storing data: {ElapsedTime}", stopwatch.Elapsed);
                           logger.BeginScope("{Empty}", string.Empty);
                       }
                       else
                       {
                           if (o.Verbose)
                           {
                               logger.LogInformation("Parsing {InputFile}", o.InputFile);
                           }
                           Stopwatch stopwatch = new Stopwatch();
                           stopwatch.Start();
                           stdf4Parser.ReadStdf4(o.InputFile);
                           stopwatch.Stop();
                           logger.LogInformation("Time to Parse: {ElapsedTime}", stopwatch.Elapsed);
                           logger.BeginScope("{Empty}", string.Empty);
                       }
                   });
        }
    }
}
