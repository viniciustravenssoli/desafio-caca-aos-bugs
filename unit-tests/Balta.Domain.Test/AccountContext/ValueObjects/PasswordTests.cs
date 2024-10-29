using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.AccountContext.ValueObjects;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class PasswordTests
{
    private const string ValidPassword = "SenhaBalta123!";
    private const string WeakPassword = "321";

    [Fact]
    public void ShouldFailIfPasswordIsNull()
    {
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(null));
    }

    [Fact]
    public void ShouldFailIfPasswordIsEmpty()
    {
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(string.Empty));
    }

    [Fact]
    public void ShouldFailIfPasswordIsWhiteSpace()
    {
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate("     "));
    }

    [Fact]
    public void ShouldFailIfPasswordLenIsLessThanMinimumChars()
    {
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(WeakPassword));
    }

    [Fact]
    public void ShouldFailIfPasswordLenIsGreaterThanMaxChars()
    {
        var longPassword = new string('a', 49);
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(longPassword));
    }

    [Fact]
    public void ShouldHashPassword()
    {
        var password = Password.ShouldCreate(ValidPassword);
        Assert.NotNull(password.Hash);
    }

    [Fact]
    public void ShouldVerifyPasswordHash()
    {
        var password = Password.ShouldCreate(ValidPassword);
        Assert.True(Password.ShouldMatch(password.Hash, ValidPassword));
    }

    [Fact]
    public void ShouldGenerateStrongPassword()
    {
        var generatedPassword = Password.ShouldGenerate();
        Assert.True(generatedPassword.Length >= 8);
    }

    [Fact]
    public void ShouldImplicitConvertToString()
    {
        var password = Password.ShouldCreate(ValidPassword);
        string passwordHash = password;

        Assert.Equal(password.Hash, passwordHash);
    }

    [Fact]
    public void ShouldReturnHashAsStringWhenCallToStringMethod()
    {
        var password = Password.ShouldCreate(ValidPassword);
        var result = password.ToString();

        Assert.Equal(password.Hash, result);
    }

    [Fact]
    public void ShouldMarkPasswordAsExpired()
    {
        var password = Password.ShouldCreate(ValidPassword);
        password.MarkAsExpired();

        Assert.True(password.IsExpired());
    }

    [Fact]
    public void ShouldFailIfPasswordIsExpired()
    {
        var password = Password.ShouldCreate(ValidPassword, expiresAtUtc: DateTime.UtcNow.AddSeconds(-1));

        Assert.True(password.IsExpired());
    }

    [Fact]
    public void ShouldMarkPasswordAsMustChange()
    {
        var password = Password.ShouldCreate(ValidPassword);
        password.MarkAsMustChange();

        Assert.True(password.IsMarkedAsMustChange());
    }

    [Fact]
    public void ShouldFailIfPasswordIsMarkedAsMustChange()
    {
        var password = Password.ShouldCreate(ValidPassword, mustChange: true);

        Assert.True(password.IsMarkedAsMustChange());
    }
}