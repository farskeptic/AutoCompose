using AutoCompose.Generator.Attributes;

namespace AutoCompose.Sample.Interfaces.Other.X02
{
    public interface ISample1X02
    {
        int Method1();
    }
    public interface ISample2X02
    {
        int Method2();
    }
    public interface ISampleX02
    {
        int MethodInterfaceNullable<T>(T sample) where T : ISample1X02?, ISample2X02?, new();
    }

    [AutoCompose(typeof(ISampleX02), "_sample")]
    public partial class SampleX02 : ISampleX02
    {
        protected readonly ISampleX02 _sample;

        public SampleX02(ISampleX02 sample)
        {
            _sample = sample;
        }
    }
}