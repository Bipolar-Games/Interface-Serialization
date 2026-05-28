#if UNITY_2022_1_OR_NEWER
using System;

namespace Bipolar.InterfaceSerialization
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class GenerateSerializedClassAttribute : Attribute
    { }
}
#endif
