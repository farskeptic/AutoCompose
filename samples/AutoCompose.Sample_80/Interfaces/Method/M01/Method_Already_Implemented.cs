using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample_80.Interfaces.Method.M01
{
    public interface ISampleM01
    {
        int Method1();
    }

    [AutoCompose(typeof(ISampleM01), "_sample")]
    public partial class SampleM01 : ISampleM01
    {
        protected readonly ISampleM01 _sample;

        public SampleM01(ISampleM01 sample)
        {
            _sample = sample;
        }

        public int Method1()
        {
            return _sample.Method1();
        }
    }
}