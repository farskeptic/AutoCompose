using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample_60.Interfaces.Method.M05
{
    public interface ISampleM05
    {
        int Method1(bool val);
    }

    [AutoCompose(typeof(ISampleM05), "_sample")]
    public partial class SampleM05 : ISampleM05
    {
        protected readonly ISampleM05 _sample;

        public SampleM05(ISampleM05 sample)
        {
            _sample = sample;
        }

        public int Method1(int val)
        {
            return _sample.Method1(false);
        }

    }
}