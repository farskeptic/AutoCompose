using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample_21.Interfaces.Method.M04
{
    public interface ISampleM04
    {
        int Method1(int val1, bool val2, string val3);
    }

    [AutoCompose(typeof(ISampleM04), "_sample")]
    public partial class SampleM04 : ISampleM04
    {
        protected readonly ISampleM04 _sample;

        public SampleM04(ISampleM04 sample)
        {
            _sample = sample;
        }

        public int Method1(bool val1, string val2, int val3)
        {
            return 55;
        }

    }
}