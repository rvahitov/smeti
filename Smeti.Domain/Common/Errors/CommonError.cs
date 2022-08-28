namespace Smeti.Domain.Common.Errors;

public static class CommonError
{
    public static IDomainError UnknownCommand(string commandName) =>
        new UnknownCommandError(commandName);
}