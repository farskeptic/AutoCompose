using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample_60.Interfaces.Method.M07
{
    public interface ISampleM07
    {
        int Method1(int nVal, bool bVal, string sVal, ISampleM07 sample);
    }

    [AutoCompose(typeof(ISampleM07), "_sample")]
    public partial class SampleM07 : ISampleM07
    {
        protected readonly ISampleM07 _sample;

        public SampleM07(ISampleM07 sample)
        {
            _sample = sample;
        }
    }
}