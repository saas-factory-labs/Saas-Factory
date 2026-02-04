using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AppBlueprint.Application.Enums;
using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.SharedKernel.SharedModels.PII;
using FluentRegex;

namespace AppBlueprint.Infrastructure.Services.PII;

public partial class RegexPIIScanner : IPIIScanner
{
    public string Name => "Regex+Luhn";

    private readonly List<PIIRegexDefinition> _definitions = new()
    {
        // [a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,20}
        new PIIRegexDefinition("Email", new Regex(Pattern.With
            .Set(Pattern.With.Letter.Digit.Literal("._%+-")).Repeat.OneOrMore
            .Literal("@")
            .Set(Pattern.With.Letter.Digit.Literal(".-")).Repeat.OneOrMore
            .Literal(".")
            .Set(Pattern.With.Letter).Repeat.Times(2, 20)
            .ToString(), RegexOptions.Compiled)),

        // (?:\+45|0045)?\s?[2-9][0-9][\s-]?[0-9]{2}[\s-]?[0-9]{2}[\s-]?[0-9]{2}
        new PIIRegexDefinition("DanishPhone", new Regex(Pattern.With
            .Group(Pattern.With.Choice(Pattern.With.Literal("+45"), Pattern.With.Literal("0045"))).Repeat.Optional
            .Whitespace.Repeat.Optional
            .Set(Pattern.With.RegEx("2-9"))
            .Digit
            .Set(Pattern.With.Whitespace.Literal("-")).Repeat.Optional
            .Digit.Repeat.Times(2)
            .Set(Pattern.With.Whitespace.Literal("-")).Repeat.Optional
            .Digit.Repeat.Times(2)
            .Set(Pattern.With.Whitespace.Literal("-")).Repeat.Optional
            .Digit.Repeat.Times(2)
            .ToString(), RegexOptions.Compiled)),

        // \+(?:[0-9]\ ?){6,14}[0-9]
        new PIIRegexDefinition("InternationalPhone", new Regex(Pattern.With
            .Literal("+")
            .Group(Pattern.With.Digit.Whitespace.Repeat.Optional).Repeat.Times(6, 14)
            .Digit
            .ToString(), RegexOptions.Compiled)),

        // \b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b
        new PIIRegexDefinition("IPv4", new Regex(Pattern.With
            .WordBoundary
            .Group(Pattern.With
                .Choice(
                    Pattern.With.Literal("25").Set(Pattern.With.RegEx("0-5")),
                    Pattern.With.Literal("2").Set(Pattern.With.RegEx("0-4")).Digit,
                    Pattern.With.Set(Pattern.With.Literal("01")).Repeat.Optional.Digit.Digit.Repeat.Optional
                )
                .Literal(".")
            ).Repeat.Times(3)
            .Choice(
                Pattern.With.Literal("25").Set(Pattern.With.RegEx("0-5")),
                Pattern.With.Literal("2").Set(Pattern.With.RegEx("0-4")).Digit,
                Pattern.With.Set(Pattern.With.Literal("01")).Repeat.Optional.Digit.Digit.Repeat.Optional
            )
            .WordBoundary
            .ToString(), RegexOptions.Compiled)),

        // \b(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}\b
        new PIIRegexDefinition("IPv6", new Regex(Pattern.With
            .WordBoundary
            .Group(Pattern.With.Set(Pattern.With.UppercaseLetter.Digit).Repeat.Times(1, 4).Literal(":")).Repeat.Times(7)
            .Set(Pattern.With.UppercaseLetter.Digit).Repeat.Times(1, 4)
            .WordBoundary
            .ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled)),

        // \b[A-Z]{2}[0-9]{2}(?:[ ]?[A-Z0-9]){12,30}\b
        new PIIRegexDefinition("IBAN", new Regex(Pattern.With
            .WordBoundary
            .Set(Pattern.With.UppercaseLetter).Repeat.Times(2)
            .Digit.Repeat.Times(2)
            .Group(Pattern.With.Literal(" ").Repeat.Optional.Set(Pattern.With.UppercaseLetter.Digit)).Repeat.Times(12, 30)
            .WordBoundary
            .ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled)),

        // \b(?:\d[ -]*?){13,16}\b
        new PIIRegexDefinition("CreditCard", new Regex(Pattern.With
            .WordBoundary
            .Group(Pattern.With.Digit.Set(Pattern.With.Literal(" -")).Repeat.ZeroOrMore.Repeat.Optional).Repeat.Times(13, 16)
            .WordBoundary
            .ToString(), RegexOptions.Compiled), ValidationFunc: ValidateLuhn),

        // \b-?(?:90(?:\.0+)?|[1-8]?\d(?:\.\d+)?),\s*-?(?:180(?:\.0+)?|(?:1[0-7]\d|\d{1,2})(?:\.\d+)?)\b
        new PIIRegexDefinition("GeoCoordinates", new Regex(Pattern.With
            .WordBoundary
            .Literal("-").Repeat.Optional
            .Choice(
                Pattern.With.Literal("90").Group(Pattern.With.Literal(".").Literal("0").Repeat.OneOrMore).Repeat.Optional,
                Pattern.With.Set(Pattern.With.RegEx("1-8")).Repeat.Optional.Digit.Group(Pattern.With.Literal(".").Digit.Repeat.OneOrMore).Repeat.Optional
            )
            .Literal(",")
            .Whitespace.Repeat.ZeroOrMore
            .Literal("-").Repeat.Optional
            .Choice(
                Pattern.With.Literal("180").Group(Pattern.With.Literal(".").Literal("0").Repeat.OneOrMore).Repeat.Optional,
                Pattern.With.Choice(
                    Pattern.With.Literal("1").Set(Pattern.With.RegEx("0-7")).Digit,
                    Pattern.With.Digit.Repeat.Times(1, 2)
                ).Group(Pattern.With.Literal(".").Digit.Repeat.OneOrMore).Repeat.Optional
            )
            .WordBoundary
            .ToString(), RegexOptions.Compiled)),

        // \b(?:[a-z0-9]{32,}|[A-Z0-9]{32,})\b
        new PIIRegexDefinition("APIKey", new Regex(Pattern.With
            .WordBoundary
            .Choice(
                Pattern.With.Set(Pattern.With.LowercaseLetter.Digit).Repeat.AtLeast(32),
                Pattern.With.Set(Pattern.With.UppercaseLetter.Digit).Repeat.AtLeast(32)
            )
            .WordBoundary
            .ToString(), RegexOptions.Compiled)),

        // \b(?:[0-3][0-9][0-1][0-9][0-9]{2}-?[0-9]{4})\b
        new PIIRegexDefinition("DanishCPR", new Regex(Pattern.With
            .WordBoundary
            .Set(Pattern.With.RegEx("0-3")).Digit
            .Set(Pattern.With.RegEx("0-1")).Digit
            .Digit.Repeat.Times(2)
            .Literal("-").Repeat.Optional
            .Digit.Repeat.Times(4)
            .WordBoundary
            .ToString(), RegexOptions.Compiled)),

        // \b\d{14,15}\b
        new PIIRegexDefinition("IMEI", new Regex(Pattern.With
            .WordBoundary
            .Digit.Repeat.Times(14, 15)
            .WordBoundary
            .ToString(), RegexOptions.Compiled), ValidationFunc: ValidateLuhn),

        // (?i)(?:password|passwd|pwd|secret|api_key|apikey|token)\s*[:=]\s*[^\s\""']{8,64}
        new PIIRegexDefinition("PotentialPasswordOrKey", new Regex(Pattern.With
            .Choice(
                Pattern.With.Literal("password"),
                Pattern.With.Literal("passwd"),
                Pattern.With.Literal("pwd"),
                Pattern.With.Literal("secret"),
                Pattern.With.Literal("api_key"),
                Pattern.With.Literal("apikey"),
                Pattern.With.Literal("token")
            )
            .Whitespace.Repeat.ZeroOrMore
            .Set(Pattern.With.Literal(":="))
            .Whitespace.Repeat.ZeroOrMore
            .NegatedSet(Pattern.With.Whitespace.Literal("\"'")).Repeat.Times(8, 64)
            .ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled))
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

                var definition = PIITypeRegistry.GetDefinition(def.Label);

                tags.Add(new PIITag
                {
                    Type = def.Label,
                    Value = match.Value,
                    Start = match.Index,
                    End = match.Index + match.Length,
                    Classification = definition.Classification,
                    IsCanonical = definition.IsCanonical
                });
            }
        }

        return Task.FromResult(tags.AsEnumerable());
    }

    private static bool ValidateLuhn(string value)
    {
        return AppBlueprint.SharedKernel.Utilities.LuhnValidator.IsValid(value);
    }

    private record PIIRegexDefinition(
        string Label,
        Regex Regex,
        Func<string, bool>? ValidationFunc = null);
}
