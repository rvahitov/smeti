using LanguageExt.Common;

namespace Smeti.Domain.Models.Common;

public static class CommonErrors
{
    public static class Codes
    {
        public const int UnknownCommand = 1_000;
    }

    public static Error CommandUnknown(string command) =>
        Error.New(Codes.UnknownCommand, $"Unknown command {command}");
}