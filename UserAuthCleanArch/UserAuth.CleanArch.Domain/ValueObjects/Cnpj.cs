using UserAuth.CleanArch.Domain.Validation;

namespace UserAuth.CleanArch.Domain.ValueObjects;

public sealed class Cnpj
{
    public string Value { get; private set; } = string.Empty;

    private Cnpj()
    {
    }

    public Cnpj(string value)
    {
        var normalizedValue = OnlyDigits(value);
        DomainExceptionValidation.When(normalizedValue.Length != 14, "CNPJ is invalid");

        Value = normalizedValue;
    }

    private static string OnlyDigits(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }
}
