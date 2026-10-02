using System.Security.Cryptography;
using CambridgeExamSystem.Application.Interfaces;

namespace CambridgeExamSystem.Infrastructure.Services;

public sealed class PasswordService : IPasswordService
{
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnopqrstuvwxyz";
    private const string Digits = "23456789";
    private const string Symbols = "!@#$%^&*?-_";

    public string GenerateTemporaryPassword(int length = 12)
    {
        if (length < 8)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Password length must be at least 8.");
        }

        var all = Upper + Lower + Digits + Symbols;
        var chars = new List<char>
        {
            Upper[RandomNumberGenerator.GetInt32(Upper.Length)],
            Lower[RandomNumberGenerator.GetInt32(Lower.Length)],
            Digits[RandomNumberGenerator.GetInt32(Digits.Length)],
            Symbols[RandomNumberGenerator.GetInt32(Symbols.Length)]
        };

        while (chars.Count < length)
        {
            chars.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);
        }

        var array = chars.ToArray();
        RandomNumberGenerator.Shuffle(array.AsSpan());
        return new string(array);
    }
}
