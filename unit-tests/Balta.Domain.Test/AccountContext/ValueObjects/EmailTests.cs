using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Extensions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class EmailTests
{
    private const string ExpectedEmailAddress = "balta@ioexample.com";
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    [Fact]
    public void ShouldLowerCaseEmail()
    {
        var email = Email.ShouldCreate(ExpectedEmailAddress.ToUpper(), _dateTimeProviderMock.Object);

        Assert.Equal(ExpectedEmailAddress, email.Address);
    }
    
    [Fact]
    public void ShouldTrimEmail()
    {
        var emailWithSpaces = $"   {ExpectedEmailAddress}   ";

        var email = Email.ShouldCreate(emailWithSpaces, _dateTimeProviderMock.Object);

        Assert.Equal(ExpectedEmailAddress, email.Address);
    }
    
    [Fact]
    public void ShouldFailIfEmailIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Email.ShouldCreate(null, _dateTimeProviderMock.Object));
    }
    
    [Fact]
    public void ShouldFailIfEmailIsEmpty()
    {
        var emptyEmailInput = string.Empty;

        Assert.Throws<ArgumentNullException>(() => Email.ShouldCreate(emptyEmailInput, _dateTimeProviderMock.Object));
    }

    [Fact]
    public void ShouldFailIfEmailIsInvalid()
    {
        var invalidEmail = "invalid-email";
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        Assert.Throws<InvalidEmailException>(() => Email.ShouldCreate(invalidEmail, dateTimeProviderMock.Object));
    }

    [Fact]
    public void ShouldPassIfEmailIsValid()
    {
        var validAddress = ExpectedEmailAddress;
        var validEmail = Email.ShouldCreate(validAddress, _dateTimeProviderMock.Object);

        Assert.Equal(ExpectedEmailAddress, validEmail.Address);
    }
    
    [Fact]
    public void ShouldHashEmailAddress()
    {
        var expectedHash = ExpectedEmailAddress.ToBase64();

        var email = Email.ShouldCreate(ExpectedEmailAddress, _dateTimeProviderMock.Object);

        Assert.Equal(expectedHash, email.Hash);
    }
    
    [Fact]
    public void ShouldExplicitConvertFromString()
    {
        var email = Email.FromString(ExpectedEmailAddress, _dateTimeProviderMock.Object);

        Assert.Equal(ExpectedEmailAddress, email.Address);
    }
    
    [Fact]
    public void ShouldExplicitConvertToString()
    {
        var email = Email.ShouldCreate(ExpectedEmailAddress, _dateTimeProviderMock.Object);
        var result = (string)email;

        Assert.Equal(ExpectedEmailAddress, result);
    }
    
    [Fact]
    public void ShouldReturnEmailWhenCallToStringMethod()
    {
        var email = Email.ShouldCreate(ExpectedEmailAddress, _dateTimeProviderMock.Object);
        var result = email.ToString();

        Assert.Equal(ExpectedEmailAddress, result);

    }
}