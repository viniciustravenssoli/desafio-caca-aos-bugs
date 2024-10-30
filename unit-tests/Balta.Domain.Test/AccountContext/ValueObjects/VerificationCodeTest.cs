using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class VerificationCodeTest
{
    private readonly Mock<IDateTimeProvider> _dateTimeProvider;
    private readonly DateTime _fixedDateTime;

    public VerificationCodeTest()
    {
        _fixedDateTime = DateTime.UtcNow;
        _dateTimeProvider = new Mock<IDateTimeProvider>();
        _dateTimeProvider.Setup(d => d.UtcNow).Returns(_fixedDateTime);
    }

    [Fact]
    public void ShouldGenerateVerificationCode()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.NotNull(verificationCode.Code);
        Assert.Equal(6, verificationCode.Code.Length);
    }

    [Fact]
    public void ShouldGenerateExpiresAtInFuture()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.True(verificationCode.ExpiresAtUtc > _dateTimeProvider.Object.UtcNow);
    }

    [Fact]
    public void ShouldGenerateVerifiedAtAsNull()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.Null(verificationCode.VerifiedAtUtc);
    }

    [Fact]
    public void ShouldBeInactiveWhenCreated()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.False(verificationCode.IsActive);
    }

    [Fact]
    public void ShouldFailIfExpired()
    {
        _dateTimeProvider.Setup(d => d.UtcNow).Returns(_fixedDateTime.AddMinutes(20));
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        verificationCode.ShouldVerify(verificationCode.Code);

        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify(verificationCode.Code));
    }

    [Fact]
    public void ShouldFailIfCodeIsInvalid()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify("INVALID"));
    }

    [Fact]
    public void ShouldFailIfCodeIsLessThanSixChars()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify("123"));
    }

    [Fact]
    public void ShouldFailIfCodeIsGreaterThanSixChars()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify("1234567"));
    }

    [Fact]
    public void ShouldFailIfIsNotActive()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        verificationCode.ShouldVerify(verificationCode.Code);

        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify(verificationCode.Code));
    }

    [Fact]
    public void ShouldFailIfIsAlreadyVerified()
    {
        _dateTimeProvider.Setup(d => d.UtcNow).Returns(_fixedDateTime);
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        verificationCode.ShouldVerify(verificationCode.Code);
        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify(verificationCode.Code));
    }

    [Fact]
    public void ShouldFailIfVerificationCodeDoesNotMatch()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify("DIFFERENT"));
    }

    [Fact]
    public void ShouldVerify()
    {
        _dateTimeProvider.Setup(d => d.UtcNow).Returns(_fixedDateTime);
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        verificationCode.ShouldVerify(verificationCode.Code);

        Assert.NotNull(verificationCode.VerifiedAtUtc);
        Assert.Null(verificationCode.ExpiresAtUtc);
        Assert.True(verificationCode.IsActive);
    }
}
