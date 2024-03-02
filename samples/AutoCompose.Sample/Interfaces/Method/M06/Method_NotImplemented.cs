using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample.Interfaces.Method.M06
{
    public interface ISampleM06
    {
        int Method1();
    }

    [AutoCompose(typeof(ISampleM06), "_sample")]
    public partial class SampleM06 : ISampleM06
    {
        protected readonly ISampleM06 _sample;

        public SampleM06(ISampleM06 sample)
        {
            _sample = sample;
        }
    }
}