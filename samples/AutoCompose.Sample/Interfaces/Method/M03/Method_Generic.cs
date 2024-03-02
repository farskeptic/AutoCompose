using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample.Interfaces.Method.M03
{
    public interface ISampleM03
    {
        int Method1<TIn, TOut>(int nVal, bool bVal, string sVal, TIn sample) where TIn : new() where TOut : new();
    }

    [AutoCompose(typeof(ISampleM03), "_sample")]
    public partial class SampleM03 : ISampleM03
    {
        protected readonly ISampleM03 _sample;

        public SampleM03(ISampleM03 sample)
        {
            _sample = sample;
        }

    }
}