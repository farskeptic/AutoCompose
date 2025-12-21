using System;

namespace AutoCompose.Generator.Attributes
{
    /// <summary>
    /// AutoComposeAttribute
    /// Use this to decorate a class that wants to implement an interface through composition.
    /// For example: If SampleClass implements ISample but has no code written to do so,
    /// decorating it with the AutoCompose attribute will cause code generation to generate
    /// pass-thru code for all of the ISample members.
    /// 
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
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoComposeAttribute : Attribute
    {
        /// <summary>
        /// TargetType
        /// </summary>
        public Type TargetType { get; protected set; }

        /// <summary>
        /// MemberName
        /// </summary>
        public string MemberName { get; protected set; }

        /// <summary>
        ///   Constructor
        ///   Takes the targetType (e.g. typeof(ISample) and the memberName
        /// </summary>
        /// <param name="targetType">e.g. typeof(ISample)</param>
        /// <param name="memberName">e.g. m_sample</param>
        public AutoComposeAttribute(Type targetType, string memberName)
        {
            TargetType = targetType;
            MemberName = memberName;
        }

        /// <summary>
        ///   Constructor
        ///   Takes the targetType as a string (e.g. "ISample") and the memberName
        ///   The type will be resolved at compile time by the source generator
        /// </summary>
        /// <param name="targetTypeName">e.g. "ISample"</param>
        /// <param name="memberName">e.g. m_sample</param>
        public AutoComposeAttribute(string targetTypeName, string memberName)
        {
            // Note: TargetType will be null when using string constructor
            // The source generator will resolve the type from the string name
            TargetType = null;
            MemberName = memberName;
        }

    }
}
