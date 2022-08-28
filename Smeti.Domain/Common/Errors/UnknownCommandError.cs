namespace Smeti.Domain.Common.Errors;

public readonly struct UnknownCommandError : IDomainError
{
    public UnknownCommandError(string commandName)
    {
        CommandName = commandName;
    }

    public string CommandName { get; }

    public void Deconstruct(out string commandName) => commandName = CommandName;
}