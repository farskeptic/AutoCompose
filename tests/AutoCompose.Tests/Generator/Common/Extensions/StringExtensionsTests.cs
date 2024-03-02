using AutoCompose.Generator.Common.Extensions;

namespace AutoCompose.Tests.Generator.Common.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("hello", "hello_suffix")]
        [InlineData("hello_suffix", "hello_suffix")]
        public void EnsureEndsWith_Succeeds(string val, string expected)
        {
            // arrange..
            // act..
            var actual = val.EnsureEndsWith("_suffix");
            // assert..
            Assert.Equal(expected, actual);
        }
    }
}
