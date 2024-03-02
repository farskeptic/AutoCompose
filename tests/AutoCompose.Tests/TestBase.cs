namespace AutoCompose.Tests
{
    public class TestBase
    {
        // jmagel - add failfast
        public TestFixture CreateTestFixture()
        {
            return new TestFixture();
        }
    }
}
