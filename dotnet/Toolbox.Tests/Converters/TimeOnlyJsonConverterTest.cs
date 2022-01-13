using System;
using System.Text.Json;
using Toolbox.Converters;
using Xunit;

namespace Toolbox.Converters;

public class TimeOnlyJsonConverterTest
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public TimeOnlyJsonConverterTest()
    {
        _jsonSerializerOptions = new JsonSerializerOptions();
        _jsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("\"22:15:42\"", 22, 15, 42, 0)]
    [InlineData("\"22:15:42.123\"", 22, 15, 42, 123)]
    public void Should_Serialize_TimeOnly(string expected, int hour, int minutes, int seconds, int millisecond)
    {
        var source = new TimeOnly(hour, minutes, seconds, millisecond);

        var result = JsonSerializer.Serialize(source, _jsonSerializerOptions);

        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(22, 15, 0, 0, "\"22:15\"")]
    [InlineData(22, 15, 42, 0, "\"22:15:42\"")]
    [InlineData(22, 15, 42, 123, "\"22:15:42.123\"")]
    public void Should_Deserialize_TimeOnly(int expectedHour, int expectedMinutes, int expectedSeconds, int expectedMillisecond, string input)
    {
        var result = JsonSerializer.Deserialize<TimeOnly>(input, _jsonSerializerOptions);

        var expected = new TimeOnly(expectedHour, expectedMinutes, expectedSeconds, expectedMillisecond);

        Assert.Equal(expected, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Should_Handle_Deserialize_Null()
    {
        var input = "null";
        var result = JsonSerializer.Deserialize<TimeOnly?>(input, _jsonSerializerOptions);

        Assert.Null(result);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("\"22\"")]
    [InlineData("\"22-15\"")]
    public void Should_Fail_When_Deserializing_Invalid_Data(string invalidInput)
        => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<DateOnly>(invalidInput, _jsonSerializerOptions));
}
