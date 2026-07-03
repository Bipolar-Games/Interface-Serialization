namespace Bipolar
{
    public static class SerializedInterfaceExtensions
    {
        public static Serialized<T> AsSerialized<T>(this T interfaceObject) where T : class => new Serialized<T>() { Value = interfaceObject };
    }
}
