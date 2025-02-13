// Copyright (c) 2025 Bill Adams. All Rights Reserved.
// Bill Adams licenses this file to you under the MIT license.
// See the license.txt file in the project root for more information.

using System;
using LavaData.Parse.Stdf4;
using System.IO;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using LavaData.Parse.Stdf4.Records;

// spell-checker:ignore stdf
namespace LavaData.Parse.Stdf.ConsoleDumper;
class Options
{
    public int DebugLevel { get; set; }

    public string InputFile { get; set; }

    public bool TimeParse { get; set; }

}

static class Program
{
    static async Task<int> Main(string[] args)
    {
        var loggerFactory = StaticProgramHelper.CreateLoggerFactory(2);
        var logger = loggerFactory.CreateLogger(nameof(Program));
        logger.LogInformation("Begin...");
        // https://github.com/dotnet/command-line-api/blob/main/docs
        var rootCommand = new RootCommand
                {
                    new Option<int>(
                        [ "--debug-level", "-d", "--log-level" ],
                        getDefaultValue: () => 3,
                        description: "0 => Errors Only, 1=> Warning, 2 => Info, 3 => Debug, 4 => Trace"),

                    new Option<string>(
                        [ "--input-file", "-i" ],
                        description: "Path to the STDF file to parse."),

                    new Option<bool>(
                        [ "--time", "--benchmark" ],
                        description: "Time parsing the file. Does not store results, useful for high-level (and questionably accurate) benchmark."),
                };

        rootCommand.Description = "Command line program for processing videos.";
        rootCommand.Handler = CommandHandler.Create
        (async (Options options) =>
            {
                await Task.CompletedTask; // Remove warning until we create async methods.

                // Replace the logger now that we know the level.
                loggerFactory = StaticProgramHelper.CreateLoggerFactory(options.DebugLevel);
                logger = loggerFactory.CreateLogger(nameof(Program));

                if (string.IsNullOrWhiteSpace(options.InputFile))
                {
                    logger.LogError("Please provide an input file w/ -i FILE");
                }
                else if (!File.Exists(options.InputFile))
                {
                    logger.LogError("ERROR: Your file {InputFile} does not exist.", options.InputFile);
                }
                else if (options.TimeParse)
                {
                    logger.LogInformation("Parsing {InputFile}", options.InputFile);

                    var stdf4Parser = new Stdf4Parser(loggerFactory, options.InputFile)
                    {
                        OnlyParse = true,
                    };

                    Stopwatch stopwatch = new Stopwatch();

                    //First, time reading the file from disk.
                    stopwatch.Start();
                    using (BinaryReader reader = new BinaryReader(File.Open(options.InputFile, FileMode.Open)))
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
                    stdf4Parser.TryParse();
                    stopwatch.Stop();
                    logger.LogInformation("         Time to Parse w/o storing data: {ElapsedTime}", stopwatch.Elapsed);
                    logger.LogInformation("{Empty}", string.Empty);
                }
                else
                {
                    logger.LogInformation("Parsing {InputFile}", options.InputFile);

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // To stop allocation and copy as the list fills up, pick a number above
                    // your maximum expected number of records in a file. This is not
                    // the number of sites, rather total records.
                    Stdf4Parser.SetInitialRecordCapacity(1_000_000);
                    var stdf4Parser = new Stdf4Parser(loggerFactory, options.InputFile);
                    stdf4Parser.TryParse();
                    stopwatch.Stop();
                    logger.LogInformation("Time to Parse: {ElapsedTime}", stopwatch.Elapsed);

                    if (!stdf4Parser.IsStateValid)
                    {
                        logger.LogWarning("Something went wrong parsing the file: {Message}", stdf4Parser.InvalidStateMessage);
                    }

                    if (stdf4Parser.IgnoredRecordTypeCount.Keys.Count > 0)
                    {
                        foreach (var kv in stdf4Parser.IgnoredRecordTypeCount)
                        {
                            logger.LogInformation("   RecordType: {RecordType}    {Count} records ignored.",
                                kv.Key, kv.Value);
                        }
                    }

                    double total = 0;
                    foreach (var r in stdf4Parser.Records)
                    {
                        if (r is PTR ptr)
                        {
                            total += ptr.TestResult;
                        }
                    }
                    logger.LogInformation("Sum of all recorded test results: {Total}", total);

                    logger.LogInformation("{Empty}", string.Empty);
                }
            });

        int returnCode = 0;
        try
        {
            // Parse the incoming args and invoke the handler
            returnCode = await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Got an exception: {ExceptionMessage}", ex.Message);
        }

        return returnCode;
    }
}
