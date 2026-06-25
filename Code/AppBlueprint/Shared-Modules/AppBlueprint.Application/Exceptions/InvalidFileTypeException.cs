namespace AppBlueprint.Application.Exceptions;

public sealed class InvalidFileTypeException : Exception
{
    public InvalidFileTypeException(string message) : base(message) { }

    public InvalidFileTypeException(string message, Exception inner) : base(message, inner) { }
}
