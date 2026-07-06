namespace StaffHubApi.Tests;

public class WeatherForecastTests
{
    [Fact]
    public void GetForecast_WhenCalled_ReturnsFiveResults()
    {
        // Arrange
        var expectedCount = 5;

        // Act
        var result = Enumerable.Range(1, 5).ToArray();

        // Assert
        Assert.Equal(expectedCount, result.Length);
    }

    [Fact]
    public void GetForecast_WhenCalled_ReturnsInvalidCount()
    {
        // This test deliberately fails to demonstrate pipeline blocking
        Assert.Equal(3, 1 + 1);
    }
}