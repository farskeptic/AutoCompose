using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample_50.Interfaces.Property.P05
{
    public interface ISampleP05
    {
        int Prop1 { get; set; }
    }

    [AutoCompose(typeof(ISampleP05), "_sample")]
    public partial class SampleP05 : ISampleP05
    {
        protected readonly ISampleP05 _sample;

        public SampleP05(ISampleP05 sample)
        {
            _sample = sample;
        }
    }
}