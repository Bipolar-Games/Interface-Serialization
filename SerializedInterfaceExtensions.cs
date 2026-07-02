using UnityEngine;

namespace Bipolar
{
    public static class SerializedInterfaceExtensions
    {
        public static Serialized<T, Object> AsSerialized<T>(this T interfaceObject) where T : class => new Serialized<T, Object>() { Value = interfaceObject };
    }
}
