using AutoCompose.Generator.Attributes;
using AutoCompose.Sample_21.Interfaces.Other.C02B;

namespace AutoCompose.Sample_21.Interfaces.Other.C02A
{
    public interface ISampleC02A
    {
        int Method1();
    }
}

namespace AutoCompose.Sample_21.Interfaces.Other.C02B
{
    public interface ISampleC02B
    {
        int Method2();
    }
}

namespace AutoCompose.Sample_21.Interfaces.Other.C02C
{
    [AutoCompose(typeof(ISampleC02B), "_sample")]
    public partial class SampleC02C : ISampleC02B // it should pick ISampleC02B
    {
        protected readonly ISampleC02B _sample;

        public SampleC02C(ISampleC02B sample)
        {
            _sample = sample;
        }

    }
}
