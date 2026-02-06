using System.Linq;

namespace AppBlueprint.SharedKernel.Utilities;

public static class LuhnValidator
{
    /// <summary>
    /// Validates a string of digits using the Luhn algorithm.
    /// </summary>
    /// <param name="value">The string to validate (non-digit characters will be filtered out).</param>
    /// <returns>True if the value is valid according to the Luhn algorithm.</returns>
    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        var digits = value.Where(char.IsDigit).Select(c => c - '0').ToArray();

        /* 
           Luhn Algorithm:
           1. From the rightmost digit, double the value of every second digit.
           2. If doubling results in a number > 9, subtract 9.
           3. Sum all digits.
           4. If sum modulo 10 is 0, then the number is valid.
        */
        if (digits.Length < 13) return false;

        int sum = 0;
        bool alternate = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int n = digits[i];
            if (alternate)
            {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alternate = !alternate;
        }

        return (sum % 10 == 0);
    }
}
