namespace Bipolar
{
    public static class SerializedInterfaceExtensions
    {
        public static Serialized<T> AsSerialized<T>(this T interfaceObject)
            where T : class
        {
            return new Serialized<T>() { Value = interfaceObject };
        }
    }
}
