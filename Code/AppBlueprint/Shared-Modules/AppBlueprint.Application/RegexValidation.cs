// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text.RegularExpressions;
// using FluentRegex;
//
// namespace AppBlueprint.Application;
//
// public class RegexValidation
// {
//     public static void GenerateEmailCombinations()
//     {
//         // Previous implementation remains the same
//         // ... [existing code for GenerateEmailCombinations()]
//     }
//
//     public static void ValidateEmail()
//     {
//         var emailRegex = Pattern.Start()
//             .Group(p => p
//                 .Group(pp => pp
//                     .CharSet(c => c
//                         .AddRange('a', 'z')
//                         .AddRange('A', 'Z')
//                         .AddRange('0', '9')
//                         .Add("!#$%&'*+/=?^_`{|}~-"))
//                     .Group(pp => pp
//                         .Literal(".")
//                         .CharSet(c => c
//                             .AddRange('a', 'z')
//                             .AddRange('A', 'Z')
//                             .AddRange('0', '9')
//                             .Add("!#$%&'*+/=?^_`{|}~-")))
//                     .ZeroOrMore())
//                 .Or()
//                 .Group(pp => pp
//                     .Literal("\"")
//                     .CharSet(c => c
//                         .AddRange('\x01', '\x08')
//                         .AddRange('\x0b', '\x0c')
//                         .AddRange('\x0e', '\x1f')
//                         .Add('\x21')
//                         .AddRange('\x23', '\x5b')
//                         .AddRange('\x5d', '\x7f'))
//                     .ZeroOrMore()
//                     .Literal("\"")))
//             .Literal("@")
//             .Group(p => p
//                 .Group(pp => pp
//                     .CharSet(c => c
//                         .AddRange('a', 'z')
//                         .AddRange('A', 'Z')
//                         .AddRange('0', '9'))
//                     .Group(ppp => ppp
//                         .CharSet(c => c
//                             .AddRange('a', 'z')
//                             .AddRange('A', 'Z')
//                             .AddRange('0', '9')
//                             .Add("-"))
//                         .ZeroOrMore()
//                         .CharSet(c => c
//                             .AddRange('a', 'z')
//                             .AddRange('A', 'Z')
//                             .AddRange('0', '9')))
//                     .Optional()
//                     .Literal(".")
//                     .OneOrMore())
//                 .CharSet(c => c
//                     .AddRange('a', 'z')
//                     .AddRange('A', 'Z')
//                     .AddRange('0', '9'))
//                 .Group(pp => pp
//                     .CharSet(c => c
//                         .AddRange('a', 'z')
//                         .AddRange('A', 'Z')
//                         .AddRange('0', '9')
//                         .Add("-"))
//                     .ZeroOrMore()
//                     .CharSet(c => c
//                         .AddRange('a', 'z')
//                         .AddRange('A', 'Z')
//                         .AddRange('0', '9')))
//                 .Optional()
//                 .Or()
//                 .Group(pp => pp
//                     .Literal("[")
//                     .Group(ppp => ppp
//                         .Group(pppp => pppp
//                             .CharSet(c => c.AddRange('0', '9')).Optional()
//                             .CharSet(c => c.AddRange('0', '9')).Optional()
//                             .CharSet(c => c.AddRange('0', '9')))
//                         .Literal(".")
//                         .Exactly(3))
//                     .Group(ppp => ppp
//                         .CharSet(c => c.AddRange('0', '9')).Optional()
//                         .CharSet(c => c.AddRange('0', '9')).Optional()
//                         .CharSet(c => c.AddRange('0', '9')))
//                     .Literal("]")))
//             .End()
//             .ToRegex(RegexOptions.IgnoreCase);
//
//         // Test some example emails
//         string[] testEmails =
//         {
//             "test@example.com",
//             "user.name+tag@[192.168.1.1]",
//             "simple@example.com",
//             "email@sub.domain.com",
//             "\"quoted.email\"@example.com",
//             "invalid-email",
//             "@missing-user.com",
//             "user@com",
//             "user@..com"
//         };
//
//         Console.WriteLine("✅ Testing Email Matches:\n");
//         foreach (var email in testEmails)
//         {
//             Console.WriteLine($"{email} → {emailRegex.IsMatch(email)}");
//         }
//     }
// }



