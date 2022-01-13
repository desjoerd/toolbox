using System;
using System.Text.Json;
using Toolbox.Converters;
using Xunit;

namespace Toolbox.Tests.Converters;

public class DateOnlyJsonConverterTest
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public DateOnlyJsonConverterTest()
    {
        _jsonSerializerOptions = new JsonSerializerOptions();
        _jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("\"2020-12-31\"", 2020, 12, 31)]
    [InlineData("\"2021-01-01\"", 2021, 01, 01)]
    [InlineData("\"2021-03-02\"", 2021, 03, 02)]
    public void Should_Serialize_DateOnly(string expected, int year, int month, int day)
    {
        var source = new DateOnly(year, month, day);

        var result = JsonSerializer.Serialize(source, _jsonSerializerOptions);

        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(2020, 12, 31, "\"2020-12-31\"")]
    [InlineData(2021, 01, 01, "\"2021-01-01\"")]
    [InlineData(2021, 03, 02, "\"2021-03-02\"")]
    public void Should_Deserialize_DateOnly(int expectedYear, int expectedMonth, int expectedDay, string input)
    {
        var result = JsonSerializer.Deserialize<DateOnly>(input, _jsonSerializerOptions);

        var expected = new DateOnly(expectedYear, expectedMonth, expectedDay);

        Assert.Equal(expected, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Should_Handle_Deserialize_Null()
    {
        var input = "null";
        var result = JsonSerializer.Deserialize<DateOnly?>(input, _jsonSerializerOptions);

        Assert.Null(result);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("\"12-31\"")]
    [InlineData("\"2021\"")]
    [InlineData("\"2021-12-31T22:59:10\"")]
    public void Should_Fail_When_Deserializing_Invalid_Data(string invalidInput)
        => Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<DateOnly>(invalidInput, _jsonSerializerOptions));
}
