using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

using Pastel;

using System.Drawing;
using System.Text;

internal class MyConsoleFormatter : ConsoleFormatter
{
    public MyConsoleFormatter() : base(nameof(MyConsoleFormatter))
    {
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {

        var colorMapping = logEntry.LogLevel switch
        {
            LogLevel.Trace => (Color.Gray, "Trac"),
            LogLevel.Debug => (Color.Gray, "Debg"),
            LogLevel.Information => (Color.Green, "Info"),
            LogLevel.Warning => (Color.Yellow, "Warn"),
            LogLevel.Error => (Color.Red, "Err"),
            LogLevel.Critical => (Color.Red, "Crit"),
            _ => (Color.Gray, logEntry.LogLevel.ToString()[0..4]),
        };
        //var logLevelString = logEntry.LogLevel.ToString().PadRight(11);
        var logLevelString = colorMapping.Item2.PadRight(4);


        var sb = new StringBuilder()
                .Append($"[{logLevelString}]".Pastel(colorMapping.Item1))
                .AppendLine($"[{logEntry.Category}] {logEntry.Formatter(logEntry.State, logEntry.Exception)}");
        textWriter.Write(sb);
    }
}

internal class MyConsoleFormatterOptions : ConsoleFormatterOptions
{
}