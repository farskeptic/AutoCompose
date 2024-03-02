
using AutoCompose.Generator.Attributes;
using AutoCompose.Sample_50.Interfaces.Other.X03A;

namespace AutoCompose.Sample_50.Interfaces.Other.X03A
{
    public interface IBaseA
    {
        int Prop1 { get; }
        int Prop2 { get; set; }
        void Method1();
        void Method2();
        int Method3(int val1, bool val2, ref string val3, out IBaseA val4);
        void ImplementedMethod();
        int OverriddenMethod(int val, ref bool bVal, out string sVal);
        T GenericMethod<T>() where T : IBaseA, new();
        T GenericMethod2<T>() where T : IBaseA?, new();
        TOut GenericMethod<TIn, TOut>(int nVal) where TIn : IBaseA, new() where TOut : IBaseA, new();

    }
}

namespace AutoCompose.Sample_50.Interfaces.Other.X03B
{
    [AutoCompose(typeof(IBaseA), "_baseA")]
    public partial class AutoComposedA : IBaseA
    {
        protected readonly IBaseA _baseA;
        public AutoComposedA(IBaseA baseA)
        {
            _baseA = baseA;
        }

    }
}
