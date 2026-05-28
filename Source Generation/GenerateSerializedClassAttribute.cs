using System;

namespace Bipolar.InterfaceSerialization
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class GenerateSerializedClassAttribute : Attribute
    { }
}
