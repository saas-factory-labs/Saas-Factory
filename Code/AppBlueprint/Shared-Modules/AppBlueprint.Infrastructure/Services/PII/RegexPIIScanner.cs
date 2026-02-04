using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AppBlueprint.Application.Enums;
using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.SharedModels.PII;

namespace AppBlueprint.Infrastructure.Services.PII;

public partial class RegexPIIScanner : IPIIScanner
{
    public string Name => "Regex+Luhn";

    private readonly List<PIIRegexDefinition> _definitions = new()
    {
        new PIIRegexDefinition(
            "Email",
            new Regex(@"
                [a-zA-Z0-9._%+-]+      # Local part
                @                     # Domain separator
                [a-zA-Z0-9.-]+         # Domain name
                \.                    # Dot
                [a-zA-Z]{2,20}         # TLD (2 to 20 chars)
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled),
            GDPRType.DirectlyIdentifiable),

        new PIIRegexDefinition(
            "DanishPhone",
            new Regex(@"
                (?:\+45|0045)?         # Optional country code
                \s?                   # Optional space
                [2-9][0-9]            # First 2 digits (exclude leading 0/1)
                [\s-]?                # Optional separator
                [0-9]{2}              # Next 2 digits
                [\s-]?                # Optional separator
                [0-9]{2}              # Next 2 digits
                [\s-]?                # Optional separator
                [0-9]{2}              # Last 2 digits
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled),
            GDPRType.DirectlyIdentifiable),

        new PIIRegexDefinition(
            "InternationalPhone",
            new Regex(@"
                \+                    # Plus prefix
                (?:[0-9]\ ?){6,14}    # 7 to 15 digits with optional spaces
                [0-9]                 # Last digit
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled),
            GDPRType.DirectlyIdentifiable),

        new PIIRegexDefinition(
            "IPv4",
            new Regex(@"
                \b                    # Word boundary
                (?:                   # Octet group
                    (?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)
                    \.                # Dot
                ){3}                  # Three octets
                (?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?) # Last octet
                \b                    # Word boundary
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled),
            GDPRType.IndirectlyIdentifiable),

        new PIIRegexDefinition(
            "IPv6",
            new Regex(@"
                \b                    # Word boundary
                (?:[A-F0-9]{1,4}:){7} # 7 groups of 1-4 hex digits + :
                [A-F0-9]{1,4}         # Last group
                \b                    # Word boundary
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Compiled),
            GDPRType.IndirectlyIdentifiable),

        new PIIRegexDefinition(
            "IBAN",
            new Regex(@"
                \b                    # Word boundary
                [A-Z]{2}[0-9]{2}      # Country code + checksum
                (?:[ ]?[A-Z0-9]){12,30} # Rest of IBAN (12 to 30 chars/digits)
                \b                    # Word boundary
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Compiled),
            GDPRType.Financial),

        new PIIRegexDefinition(
            "CreditCard",
            new Regex(@"
                \b                    # Word boundary
                (?:\d[ -]*?){13,16}   # 13 to 16 digits with optional spaces/dashes
                \b                    # Word boundary
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled),
            GDPRType.Financial,
            ValidationFunc: ValidateLuhn),

        new PIIRegexDefinition(
            "GeoCoordinates",
            new Regex(@"
                \b                    # Word boundary
                -?(?:90(?:\.0+)?|[1-8]?\d(?:\.\d+)?) # Latitude range check
                ,\s*                  # Separator
                -?(?:180(?:\.0+)?|(?:1[0-7]\d|\d{1,2})(?:\.\d+)?) # Longitude range check
                \b                    # Word boundary
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled),
            GDPRType.IndirectlyIdentifiable),

        new PIIRegexDefinition(
            "APIKey",
            new Regex(@"
                \b                    # Word boundary
                (?:[a-z0-9]{32,}|[A-Z0-9]{32,}) # Entropy-based key (32+ chars)
                \b                    # Word boundary
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled),
            GDPRType.SensitiveMiscellaneous),

        new PIIRegexDefinition(
            "DanishCPR",
            new Regex(@"
                \b                    # Word boundary
                (?:[0-3][0-9][0-1][0-9][0-9]{2} # Birthday part (DDMMYY)
                -?                    # Optional separator
                [0-9]{4})             # Last 4 digits (serial)
                \b                    # Word boundary
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled),
            GDPRType.DirectlyIdentifiable)
    };

    public Task<IEnumerable<PIITag>> ScanAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult(Enumerable.Empty<PIITag>());
        }

        var tags = new List<PIITag>();

        foreach (var def in _definitions)
        {
            var matches = def.Regex.Matches(text);
            foreach (Match match in matches)
            {
                if (def.ValidationFunc != null && !def.ValidationFunc(match.Value))
                {
                    continue;
                }

                tags.Add(new PIITag
                {
                    Type = def.Type,
                    Value = match.Value,
                    Start = match.Index,
                    End = match.Index + match.Length,
                    Classification = def.Classification
                });
            }
        }

        return Task.FromResult(tags.AsEnumerable());
    }

    private static bool ValidateLuhn(string value)
    {
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

    private record PIIRegexDefinition(
        string Type,
        Regex Regex,
        GDPRType Classification,
        Func<string, bool>? ValidationFunc = null);
}
