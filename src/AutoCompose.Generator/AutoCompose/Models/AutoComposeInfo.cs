namespace AutoCompose.Generator.AutoCompose.Models
{
    /// <summary>
    /// The argument values of the AutoComposeAttribute, as string values.
    /// e.g. for:
    ///  [AutoCompose(typeof(ISample), "_sample")]
    ///  public class SampleClass : ISample
    ///  {
    ///      protected readonly ISample _sample;
    ///
    ///      public SampleClass(ISample sample)
    ///      {
    ///          _sample = sample;
    ///      }
    ///  }
    ///  TargetType = "ISample"
    ///  MemberName = "_sample"
    /// </summary>
    public class AutoComposeInfo
    {
        public string TargetType { get; protected set; }
        public string MemberName { get; protected set; }

        public AutoComposeInfo(string targetType, string memberName)
        {
            TargetType = targetType;
            MemberName = memberName;
        }

    }
}
