﻿//HintName: AutoCompose/Sample/M02..g.cs
// <auto-generated> 
// WARNING: THIS CODE IS AUTO-GENERATED AT COMPILE-TIME.  ANY CHANGES WILL BE OVERWRITTEN ON NEXT COMPILE.
// </auto-generated> 



namespace AutoCompose.Sample.M02
{
    public partial class SampleM02
    {


        public virtual TOut Method1<TIn, TOut>(int nVal, bool bVal, string sVal, TIn sample) where TIn : notnull, new() where TOut : class?, new()
        {
            return _sample.Method1<TIn, TOut>(nVal, bVal, sVal, sample);
        }


        public virtual TOut Method2<TIn, TOut>(int nVal, bool bVal, string sVal, TIn sample) where TIn : notnull, new() where TOut : struct
        {
            return _sample.Method2<TIn, TOut>(nVal, bVal, sVal, sample);
        }


        public virtual int MethodUnmanaged<T>(T sample) where T : unmanaged
        {
            return _sample.MethodUnmanaged<T>(sample);
        }


        public virtual int MethodInterface<T>(T sample) where T : ISampleM02
        {
            return _sample.MethodInterface<T>(sample);
        }


        public virtual int MethodInterfaceNullable<T>(T sample) where T : ISampleM02?, new()
        {
            return _sample.MethodInterfaceNullable<T>(sample);
        }


        public virtual int MethodClass<T>(T sample) where T : SampleM02
        {
            return _sample.MethodClass<T>(sample);
        }


        public virtual int MethodClassNullable<T>(T sample) where T : SampleM02?
        {
            return _sample.MethodClassNullable<T>(sample);
        }


        public virtual int MethodTU<T, U>(T sample, U sample2) where T : U where U : new()
        {
            return _sample.MethodTU<T, U>(sample, sample2);
        }


        public virtual int MethodStruct<T>(T sample) where T : struct
        {
            return _sample.MethodStruct<T>(sample);
        }


    }
}