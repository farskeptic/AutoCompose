using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample_20.Interfaces.Other.C01A
{
    public interface ISampleC01A
    {
        int Method1();
    }

    [AutoCompose(typeof(ISampleC01A), "_sample")]
    public partial class SampleC01A : ISampleC01A
    {
        protected readonly ISampleC01A _sample;

        public SampleC01A(ISampleC01A sample)
        {
            _sample = sample;
        }

    }
}

namespace AutoCompose.Sample_20.Interfaces.Other.C01B
{
    public interface ISampleC01B
    {
        int Method1();
    }

    [AutoCompose(typeof(ISampleC01B), "_sample")]
    public partial class SampleC01B : ISampleC01B
    {
        protected readonly ISampleC01B _sample;

        public SampleC01B(ISampleC01B sample)
        {
            _sample = sample;
        }

    }
}