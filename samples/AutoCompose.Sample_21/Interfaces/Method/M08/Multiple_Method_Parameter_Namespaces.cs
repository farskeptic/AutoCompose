using AutoCompose.Generator.Attributes;
using AutoCompose.Sample_21.Interfaces.Method.M08A;
using AutoCompose.Sample_21.Interfaces.Method.M08B;
using AutoCompose.Sample_21.Interfaces.Method.M08C;
using AutoCompose.Sample_21.Interfaces.Method.M08E;

namespace AutoCompose.Sample_21.Interfaces.Method.M08A
{
    public interface ISample1M08A
    {
        int Method1();
    }

    public class Sample1M08A : ISample1M08A
    {
        public int Method1()
        {
            return 55;
        }
    }
}

namespace AutoCompose.Sample_21.Interfaces.Method.M08B
{
    public interface ISample2M08B
    {
        int Method2();
    }

    public class Sample2M08B : ISample2M08B
    {
        public int Method2()
        {
            return 56;
        }
    }
}

namespace AutoCompose.Sample_21.Interfaces.Method.M08C
{

    public interface ISample3M08C
    {
        int Method2(ISample1M08A sample1, ISample2M08B sample2);
    }

}

namespace AutoCompose.Sample_21.Interfaces.Method.M08D
{

    [AutoCompose(typeof(ISample3M08C), "_sample3")]
    public partial class Sample3M08D : ISample3M08C
    {
        protected readonly ISample3M08C _sample3;

        public Sample3M08D(ISample3M08C sample3)
        {
            _sample3 = sample3;
        }

    }
}

namespace AutoCompose.Sample_21.Interfaces.Method.M08E
{

    public interface ISample4M08E
    {
        int Method3(Sample1M08A sample1, ISample2M08B sample2);
    }

}

namespace AutoCompose.Sample_21.Interfaces.Method.M08F
{

    [AutoCompose(typeof(ISample4M08E), "_sample4")]
    public partial class SampleM08F : ISample4M08E
    {
        protected readonly ISample4M08E _sample4;

        public SampleM08F(ISample4M08E sample4)
        {
            _sample4 = sample4;
        }

    }
}