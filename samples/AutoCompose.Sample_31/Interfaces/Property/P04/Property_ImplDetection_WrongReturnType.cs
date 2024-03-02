// Note: C#.NET has trouble with this one - doesn't detect the mismatches

//using AutoCompose.Generator.Attributes;

//namespace AutoCompose.Sample_31.P04
//{
//    public interface ISampleP04
//    {
//        int Prop1 { get; set; }
//    }

//    [AutoCompose(typeof(ISampleP04), "_sample")]
//    public partial class SampleP04 : ISampleP04
//    {
//        protected readonly ISampleP04 _sample;

//        public bool Prop1 { get; set; }

//        public SampleP04(ISampleP04 sample)
//        {
//            _sample = sample;
//        }
//    }
//}