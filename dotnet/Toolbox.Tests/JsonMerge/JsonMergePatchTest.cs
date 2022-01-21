using System.Text.Json;
using FluentAssertions;
using Toolbox.JsonMerge;
using Xunit;

namespace Toolbox.Tests.JsonMerge
{
    public class JsonMergePatchTest
    {
        [Fact]
        public void ShouldMergePatchSimpleJsonObject()
        {
            var patchJson = @"
{
  ""Name"": ""DeSjoerd"",
  ""IntValue"": 12,
  ""BoolValue"": true
}
";
            var patchDocument = JsonSerializer.Deserialize<JsonMergePatchDocument<SimpleObject>>(patchJson);

            var source = new SimpleObject
            {
                Name = "Sjoerd",
                StringValue = "abc"
            };

            var result = patchDocument.CreatePatchedValue(source);

            Assert.Equal("DeSjoerd", result.Name);
            Assert.Equal("abc", result.StringValue);
            Assert.Equal(12, result.IntValue);
            Assert.Equal(true, result.BoolValue);
        }

        [Fact]
        public void ShouldMergePatchNestedJsonObject()
        {
            var patchJson = @"
{
  ""SimpleObject"": {
    ""Name"": ""DeSjoerd""
  },
  ""Nested"": {
    ""SimpleObject"": {
      ""IntValue"": 42
    },
    ""Nested"": {
      ""SimpleObject"": {
        ""Name"": ""DeSjoerd""
      },
      ""Nested"": null
    }
  }
}
";
            var source = new NestedObject
            {
                SimpleObject = new SimpleObject
                {
                    Name = "Sjoerd",
                    IntValue = 1,
                },
                Nested = new NestedObject
                {
                    SimpleObject = null,
                    Nested = new NestedObject
                    {
                        SimpleObject = new SimpleObject
                        {
                            Name = "Sjoerd"
                        },
                        Nested = new NestedObject
                        {
                            SimpleObject = new SimpleObject
                            {
                                Name = "Removed"
                            }
                        }
                    }
                }
            };

            var patchDocument = JsonSerializer.Deserialize<JsonMergePatchDocument<NestedObject>>(patchJson);

            var result = patchDocument.CreatePatchedValue(source);

            var expected = new NestedObject
            {
                SimpleObject = new SimpleObject
                {
                    Name = "DeSjoerd",
                    IntValue = 1
                },
                Nested = new NestedObject
                {
                    SimpleObject = new SimpleObject
                    {
                        IntValue = 42
                    },
                    Nested = new NestedObject
                    {
                        SimpleObject = new SimpleObject
                        {
                            Name = "DeSjoerd"
                        },
                        Nested = null
                    }
                }
            };

            result.Should().BeEquivalentTo(expected);
        }

        private class SimpleObject
        {
            public string? Name { get; set; }
            public string? StringValue { get; set; }
            public int? IntValue { get; set; }
            public bool? BoolValue { get; set; }
        }

        private class NestedObject
        {
            public SimpleObject? SimpleObject { get; set; }

            public NestedObject? Nested { get; set; }
        }
    }
}
