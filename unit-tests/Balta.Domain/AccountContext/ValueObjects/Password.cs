using System.Security.Cryptography;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.ValueObjects;

namespace Balta.Domain.AccountContext.ValueObjects;

public record Password : ValueObject
{
    #region Constants

    private const int MinLength = 8;
    private const int MaxLength = 48;
    private const string Valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private const string Special = "!@#$%ˆ&*(){}[];";

    #endregion

    #region Constructors

    private Password(string hash, bool mustChange, DateTime? expiresAtUtc)
    {
        Hash = hash;
        MustChange = mustChange;
        ExpiresAtUtc = expiresAtUtc;
    }

    #endregion

    #region Factories

    public static Password ShouldCreate(string plainText, bool mustChange = false, DateTime? expiresAtUtc = null)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new InvalidPasswordException("Password cannot be null or empty");

        if (plainText.Length < MinLength)
            throw new InvalidPasswordException($"Password should have at least {MinLength} characters");

        if (plainText.Length > MaxLength)
            throw new InvalidPasswordException($"Password should have less than {MaxLength} characters");

        var hash = ShouldHashPassword(plainText);
        
        return new Password(hash, mustChange, expiresAtUtc);
    }

    #endregion

    #region Properties

    public string Hash { get; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public bool MustChange { get; private set; }

    #endregion

    #region Public Methods

    public void MarkAsExpired()
    {
        ExpiresAtUtc = DateTime.UtcNow;
    }

    public void MarkAsMustChange()
    {
        MustChange = true;
    }

    public static string ShouldGenerate(
        short length = 16,
        bool includeSpecialChars = true,
        bool upperCase = true)
    {
        var chars = includeSpecialChars ? (Valid + Special) : Valid;
        var startRandom = upperCase ? 26 : 0;
        var index = 0;
        var res = new char[length];
        var rnd = new Random();

        while (index < length)
            res[index++] = chars[rnd.Next(startRandom, chars.Length)];

        return new string(res);
    }

    public static bool ShouldMatch(
        string hash,
        string password,
        short keySize = 32,
        int iterations = 10000,
        char splitChar = '.')
    {
        password += Configuration.Api.PasswordSalt;

        var parts = hash.Split(splitChar, 3);
        if (parts.Length != 3)
            return false;

        var hashIterations = Convert.ToInt32(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        if (hashIterations != iterations)
            return false;

        using var algorithm = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256);
        var keyToCheck = algorithm.GetBytes(keySize);

        return keyToCheck.SequenceEqual(key);
    }

    public bool IsExpired() => ExpiresAtUtc.HasValue && ExpiresAtUtc.Value <= DateTime.UtcNow;

    public bool IsMarkedAsMustChange() => MustChange;

    #endregion

    #region Private Methods

    private static string ShouldHashPassword(
        string password,
        short saltSize = 16,
        short keySize = 32,
        int iterations = 10000,
        char splitChar = '.')
    {
        if (string.IsNullOrEmpty(password))
            throw new InvalidPasswordException("Password should not be null or empty");

        password += Configuration.Api.PasswordSalt;

        using var algorithm = new Rfc2898DeriveBytes(
            password,
            saltSize,
            iterations,
            HashAlgorithmName.SHA256);
        var key = Convert.ToBase64String(algorithm.GetBytes(keySize));
        var salt = Convert.ToBase64String(algorithm.Salt);

        return $"{iterations}{splitChar}{salt}{splitChar}{key}";
    }

    #endregion

    #region Operators

    public static implicit operator Password(string plainTextPassword) => new(plainTextPassword);
    public static implicit operator string(Password password) => password.Hash;

    #endregion

    #region Overrides

    public override string ToString() => Hash;

    #endregion
}