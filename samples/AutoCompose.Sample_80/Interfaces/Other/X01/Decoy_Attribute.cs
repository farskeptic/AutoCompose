//// Note: C#.NET gives "does not implement member" error

//using AutoCompose.Sample_80.X01A;

//namespace AutoCompose.Sample_80.X01A
//{
//    public class DecoyAttribute : Attribute
//    {
//        protected readonly Type _targetType;
//        protected readonly string _memberName;
//        public DecoyAttribute(Type targetType, string memberName)
//        {
//            _targetType = targetType;
//            _memberName = memberName;
//        }

//        public Type GetTargetType()
//        {
//            return _targetType;
//        }

//        public string GetMemberName()
//        {
//            return _memberName;
//        }
//    }
//}

//namespace AutoCompose.Sample_80.X01B
//{
//    public interface ISampleX01B
//    {
//        int Prop1 { get; set; }
//    }

//    [Decoy(typeof(ISampleX01B), "_sample")]
//    public class SampleX01B : ISampleX01B
//    {
//        protected readonly ISampleX01B _sample;

//        public SampleX01B(ISampleX01B sample)
//        {
//            _sample = sample;
//        }
//    }
//}
