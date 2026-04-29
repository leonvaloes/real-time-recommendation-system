namespace CleanArch.Domain.Abstractions;

public interface IPerson
{
    Guid Id { get; }
    string GetIdentifier();
    string GetDisplayName();
}
