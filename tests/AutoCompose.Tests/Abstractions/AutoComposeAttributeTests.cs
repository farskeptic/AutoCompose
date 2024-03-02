using AutoCompose.Generator.Attributes;

namespace AutoCompose.Tests.Abstractions
{
    public class AutoComposeAttributeTests
    {
        /// <summary>
        /// This test is for coverage only, and gives credence to the idea that AutoProperties could be done like in typescript
        /// e.g. public class AutoComposeAttribute(public int a, public int b)
        /// </summary>
        [Fact]
        public void AttributeProperties_Coverage()
        {
            // arrange..
            // act..
            var target = new AutoComposeAttribute(typeof(AutoComposeAttributeTests), "test");
            // assert..
            Assert.Equal("test", target.MemberName);
            Assert.Equal(typeof(AutoComposeAttributeTests), target.TargetType);
        }
    }
}
