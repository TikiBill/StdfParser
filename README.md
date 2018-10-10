# STDF4 Reader/Writer

A c#/.NET Core library to read and write STDF4 files. This just makes a list/stream
of records which another library or program should consume and process
into the desired data structures.

## Current State

The initial version of this library was developed against some example
STDF files, including some non-public ones. While the library parses
the files without issue, currently there are a few limitations:

* Lacking Unit Tests
* Writing out an STDF4 is untested and broken.
* A few record types are unsupported (I lacked examples to develop against.)

Of course, I welcome contributions esp. if you have an STDF file with the
missing record types.
