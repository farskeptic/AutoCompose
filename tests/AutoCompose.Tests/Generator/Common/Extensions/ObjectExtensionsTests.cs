using AutoCompose.Generator.Common.Extensions;
using AutoCompose.Generator.Common.Models;

namespace AutoCompose.Tests.Generator.Common.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void GuardNull_Returns_Value()
        {
            // arrange..
            var val = new PropertyModel();
            // act..
            var actual = val.GuardNull();
            // assert..
            Assert.Equal(val, actual);
        }

        [Fact]
        public void GuardNull_Throws_On_Null()
        {
            // arrange..
            object? val = null;
            // act..
            // assert..
            Assert.Throws<ArgumentNullException>(() => val.GuardNull());
        }

        [Fact]
        public void GuardType_Returns_Value()
        {
            // arrange..
            var val = new PropertyModel();
            // act..
            var actual = val.GuardType<PropertyModel>();
            // assert..
            Assert.Equal(val, actual);
        }

        [Fact]
        public void GuardNull_Type_On_Mismatch()
        {
            // arrange..
            var val = new PropertyModel();
            // act..
            // assert..
            Assert.Throws<ArgumentException>(() => val.GuardType<MethodModel>());
        }
    }
}
