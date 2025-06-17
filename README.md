# STDF4 Reader/Writer

A c#/.NET Core library to read and write STDF4 files. This just makes a list/stream
of records which another library or program should consume and process
into the desired data structures.

> NOTE: This is in early development and as such the API and behavior may change.

## Current State

The initial version of this library was developed against some example
STDF files, including some non-public ones. While the library parses
the files without issue, currently there are a few limitations:

* Lacking Unit Tests
* Writing out an STDF4 is untested and broken.
* A few record types are unsupported (I lacked examples to develop against.)

Of course, I welcome contributions esp. if you have an STDF file with the
missing record types.

## Example

The program in `src/stdf4Parser.IgnoredRecordTypeCount` gives a good example
of parsing a file. There is not a lot to the parsing part besides checking
the value of `IsStateValid`.
Most of the work will be the program you write to go through the `Records` property
and transforming the data into something useful.

```csharp
    // To stop allocation and copy as the list fills up, pick a number above
    // your maximum expected number of records in a file. This is not
    // the number of sites, rather total records.
    Stdf4Parser.SetInitialRecordCapacity(1_000_000);
    var stdf4Parser = new Stdf4Parser(loggerFactory, options.InputFile);
    stdf4Parser.TryParse();

    if (!stdf4Parser.IsStateValid)
    {
        logger.LogWarning("Something went wrong parsing the file: {Message}", stdf4Parser.InvalidStateMessage);
    }

    // Print out any records for which we have a case statement, but no strongly typed class.
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

```

## END OF LINE