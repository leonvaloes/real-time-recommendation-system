using UserAuth.CleanArch.Domain.Validation;

namespace UserAuth.CleanArch.Domain.ValueObjects;

public sealed class Cpf
{
    public string Value { get; private set; } = string.Empty;

    private Cpf()
    {
    }

    public Cpf(string value)
    {
        var normalizedValue = OnlyDigits(value);
        DomainExceptionValidation.When(normalizedValue.Length != 11, "CPF is invalid");

        Value = normalizedValue;
    }

    private static string OnlyDigits(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }
}
