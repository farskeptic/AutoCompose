using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample_80.Interfaces.Property.P02
{
    public interface ISampleP02
    {
        int Prop1 { get; set; }
    }

    [AutoCompose(typeof(ISampleP02), "_sample")]
    public partial class SampleP02 : ISampleP02
    {
        protected readonly ISampleP02 _sample;

        public int Prop1 { get; set; }

        public SampleP02(ISampleP02 sample)
        {
            _sample = sample;
        }
    }
}