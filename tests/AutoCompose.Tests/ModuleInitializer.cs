using System.Runtime.CompilerServices;

namespace AutoCompose.Tests
{
    // As per: https://github.com/VerifyTests/Verify.SourceGenerators
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifySourceGenerators.Enable();
        }
    }
}
