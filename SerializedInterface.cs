using System;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Bipolar
{
    [Serializable]
    public class Serialized<TInterface> : Serialized<TInterface, Object>
        where TInterface : class
    { }

    internal interface ISerializedInterface
    {
        Object SerializedObject { get; }
    }

    [Serializable]
    public class Serialized<TInterface, TSerialized> : ISerializedInterface, ISerializationCallbackReceiver, IEquatable<TInterface>
        where TInterface : class
        where TSerialized : Object
    {
        [SerializeField]
        private TSerialized serializedObject;

        private TInterface _value;
        public virtual TInterface Value
        {
            get
            {
                _value ??= serializedObject as TInterface;
                return _value;
            }

            set
            {
                SetValue(value);
            }
        }

        private void SetValue(TInterface value)
        {
            if (value == null)
            {
                serializedObject = null;
                _value = null;
            }
            else if (value is TSerialized @object)
            {
                serializedObject = @object;
                _value = value;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public Type InterfaceType => typeof(TInterface);

        Object ISerializedInterface.SerializedObject => serializedObject;

        public override string ToString() => Value?.ToString() ?? "null";

        public static bool operator !=(TInterface x, Serialized<TInterface, TSerialized> y) => !y.Equals(x);
        public static bool operator ==(TInterface x, Serialized<TInterface, TSerialized> y) => y.Equals(x);
        public static bool operator !=(Serialized<TInterface, TSerialized> x, TInterface y) => !x.Equals(y);
        public static bool operator ==(Serialized<TInterface, TSerialized> x, TInterface y) => x.Equals(y); 

        public bool Equals(TInterface other)
        {
            if (other is TSerialized)
                return serializedObject == other;

            if (other is ISerializedInterface ySerialized)
                return serializedObject == ySerialized.SerializedObject;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is ISerializedInterface ifaceSerialized)
                return serializedObject == ifaceSerialized.SerializedObject;

            if (obj is TInterface iface)
                return Equals(iface);
            
            return false;
        }

        public override int GetHashCode() => serializedObject?.GetHashCode() ?? 0;

        void ISerializationCallbackReceiver.OnBeforeSerialize() => _value = null;
        void ISerializationCallbackReceiver.OnAfterDeserialize() => _value = null;
    }

    public static class InterfaceExtensions
    {
        public static Serialized<T> AsSerialized<T>(this T interfaceObject) where T : class => new Serialized<T>() { Value = interfaceObject };
    }
}
