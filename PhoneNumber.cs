namespace DeliveryExpress.Services;

public static class PhoneNumber
{
    public static string Digits(string? value) =>
        new string((value ?? "").Where(char.IsDigit).ToArray());

    public static string Format(string digits)
    {
        if (digits.Length == 11)
            return $"({digits[..2]}) {digits[2..7]}-{digits[7..]}";
        if (digits.Length == 10)
            return $"({digits[..2]}) {digits[2..6]}-{digits[6..]}";
        return digits;
    }
}
