using CardiacPatientMonitoring.Api.Services;

namespace CardiacPatientMonitoring.Tests;

public class HeartRateServiceTests
{
    // A normal heart rate should be accepted.
    [Fact]
    public void IsHeartRateValid_ShouldReturnTrue_WhenHeartRateIsNormal()
    {
        // Arrange
        var service = new HeartRateService();
        var heartRate = 80;

        // Act
        var result = service.IsHeartRateValid(heartRate);

        // Assert
        Assert.True(result);
    }

    // The lowest value in our accepted range should be valid.
    [Fact]
    public void IsHeartRateValid_ShouldReturnTrue_WhenHeartRateIs40()
    {
        // Arrange
        var service = new HeartRateService();
        var heartRate = 40;

        // Act
        var result = service.IsHeartRateValid(heartRate);

        // Assert
        Assert.True(result);
    }

    // A heart rate above the accepted range should be rejected.
    [Fact]
    public void IsHeartRateValid_ShouldReturnFalse_WhenHeartRateIsTooHigh()
    {
        // Arrange
        var service = new HeartRateService();
        var heartRate = 250;

        // Act
        var result = service.IsHeartRateValid(heartRate);

        // Assert
        Assert.False(result);
    }
    // Theory lets us test several input values using the same test logic.
[Theory]
[InlineData(40, true)]
[InlineData(100, true)]
[InlineData(200, true)]
[InlineData(250, false)]
public void IsHeartRateValid_ShouldReturnExpectedResult(
    int heartRate,
    bool expected)
{
    // Arrange
    var service = new HeartRateService();

    // Act
    var result = service.IsHeartRateValid(heartRate);

    // Assert
    Assert.Equal(expected, result);
}
}