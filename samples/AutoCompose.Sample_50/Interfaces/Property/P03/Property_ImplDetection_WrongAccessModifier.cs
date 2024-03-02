// Note: C#.NET gives compile error since wrong return type doesn't satify the interface

//using AutoCompose.Generator.Attributes;

//namespace AutoCompose.Sample_50.P03
//{
//    public interface ISampleP03
//    {
//        int Prop1 { get; set; }
//    }

//    [AutoCompose(typeof(ISampleP03), "_sample")]
//    public partial class SampleP03 : ISampleP03
//    {
//        protected readonly ISampleP03 _sample;

//        protected int Prop1 { get; set; }

//        public SampleP03(ISampleP03 sample)
//        {
//            _sample = sample;
//        }
//    }
//}