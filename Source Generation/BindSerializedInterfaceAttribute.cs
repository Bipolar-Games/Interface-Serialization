#if UNITY_2022_1_OR_NEWER
using System;

namespace Bipolar.InterfaceSerialization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class BindSerializedInterfaceAttribute : Attribute
    {
        public Type InterfaceType { get; }

        public BindSerializedInterfaceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}
#endif
 