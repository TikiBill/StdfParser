using Microsoft.Extensions.Logging;
using Serilog;

#pragma warning disable S125 // Commented out code.

// spell-checker:ignore stdf serilog
namespace LavaData.Parse.Stdf.ConsoleDumper;
public static class StaticProgramHelper
{
    public static ILoggerFactory CreateLoggerFactory(int debugLevel)
    {
        // Assign the static logger too for the occasions when we
        // use it, e.g. in static code and some debugging code.
        Serilog.Log.Logger = GetSerilogLogger(IntToLogLevel(debugLevel));
        return new LoggerFactory().AddSerilog(logger: Serilog.Log.Logger, dispose: true);
    }

    internal static LogLevel IntToLogLevel(int debugLevel)
    {
        switch (debugLevel)
        {
            case 0:
                return LogLevel.Error;
            case 1:
                return LogLevel.Warning;
            case 2:
                return LogLevel.Information;
            case 3:
                return LogLevel.Debug;
            case 4:
                return LogLevel.Trace;
            default:
                return LogLevel.Information;
        }
    }

    public static Serilog.Core.Logger GetSerilogLogger(LogLevel logLevel)
    {
        // var outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";
        var outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext:l} - {Message:l}{NewLine}{Exception}";
        switch (logLevel)
        {
            case LogLevel.Error:
                return new Serilog.LoggerConfiguration()
                    .MinimumLevel.Error()
                    .WriteTo.Console(outputTemplate: outputTemplate)
                    .CreateLogger();

            case LogLevel.Warning:
                return new Serilog.LoggerConfiguration()
                    .MinimumLevel.Warning()
                    .WriteTo.Console(outputTemplate: outputTemplate)
                    .CreateLogger();

            case LogLevel.Debug:
                return new Serilog.LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(outputTemplate: outputTemplate)
                    .CreateLogger();

            case LogLevel.Trace:
                return new Serilog.LoggerConfiguration()
                    .MinimumLevel.Verbose() // Remember verbose for serilog is what most others call trace.
                    .WriteTo.Console(outputTemplate: outputTemplate)
                    .CreateLogger();

            default:
                // This covers LogLevel.Information
                return new Serilog.LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                        outputTemplate: outputTemplate,
                        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate)
                    .CreateLogger();
        }
    }
}
