using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "RCS1090: Add call to 'ConfigureAwait' (or vice versa).",
    Justification = "No synchronisation context in ASP.NET Core")]
