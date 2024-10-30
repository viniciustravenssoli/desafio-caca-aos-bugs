using Balta.Domain.SharedContext.Extensions;
using System.Text;

namespace Balta.Domain.Test.SharedContext.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ShouldGenerateBase64FromString()
    {
        // Arrange
        string originalString = "Hello, World!";
        string expectedBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(originalString));

        // Act
        string actualBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(originalString));

        // Assert
        Assert.Equal(expectedBase64, actualBase64);
    }
}