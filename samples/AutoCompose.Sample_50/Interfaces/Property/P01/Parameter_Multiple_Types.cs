using AutoCompose.Generator.Attributes;
using AutoCompose.Sample_50.Interfaces.Property.P01B;

namespace AutoCompose.Sample_50.Interfaces.Property.P01A
{
    public interface ISampleP01A
    {
        int MethodA();
    }

    public interface IParameterSample
    {
        int MethodA();
        int MethodA(IParameterSample parameterSample);
    }
}

namespace AutoCompose.Sample_50.Interfaces.Property.P01B
{
    public interface ISampleP01B
    {
        int MethodB();
        int MethodB(IParameterSample parameterSample);
    }

    public interface IParameterSample
    {
        int MethodB();
    }

}

namespace AutoCompose.Sample_50.Interfaces.Property.P01C
{
    [AutoCompose(typeof(ISampleP01B), "_sample")]
    public partial class SampleP01C : ISampleP01B // it should pick ISampleP01B
    {
        protected readonly ISampleP01B _sample;

        public SampleP01C(ISampleP01B sample)
        {
            _sample = sample;
        }

    }
}
