using System;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Bipolar
{
    [Serializable]
    public abstract class SerializedInterface<TInterface> : SerializedInterface<TInterface, Object>
        where TInterface : class
    { }

    internal interface ISerializedInterface
    {
        Object SerializedObject { get; }
    }

    [Serializable]
    public abstract class SerializedInterface<TInterface, TSerialized> : ISerializedInterface, ISerializationCallbackReceiver, IEquatable<TInterface>
        where TInterface : class
        where TSerialized : Object
    {
        [SerializeField]
        protected Serialized<TInterface, TSerialized> serializedObject;

        public Type InterfaceType => typeof(TInterface);

        Object ISerializedInterface.SerializedObject => serializedObject.serializedObject;

        public override string ToString() => serializedObject.ToString();

        public static bool operator !=(TInterface x, SerializedInterface<TInterface, TSerialized> y) => !y.Equals(x);
        public static bool operator ==(TInterface x, SerializedInterface<TInterface, TSerialized> y) => y.Equals(x);
        public static bool operator !=(SerializedInterface<TInterface, TSerialized> x, TInterface y) => !x.Equals(y);
        public static bool operator ==(SerializedInterface<TInterface, TSerialized> x, TInterface y) => x.Equals(y);

        public bool Equals(TInterface other) => serializedObject.Equals(other);

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

    public struct Serialized<TInterface, TSerialized> : ISerializedInterface, ISerializationCallbackReceiver, IEquatable<TInterface>
        where TInterface : class
        where TSerialized : Object
    {
        [SerializeField]
        internal TSerialized serializedObject;

        private TInterface _value;
        public TInterface Value
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

        private void SetValue(TInterface newValue)
        {
            if (newValue == null)
            {
                serializedObject = null;
                _value = null;
            }
            else if (newValue is TSerialized @object)
            {
                serializedObject = @object;
                _value = newValue;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public readonly Type InterfaceType => typeof(TInterface);

        readonly Object ISerializedInterface.SerializedObject => serializedObject;

        public override string ToString() => Value?.ToString() ?? "null";

        public static bool operator !=(TInterface x, Serialized<TInterface, TSerialized> y) => !y.Equals(x);
        public static bool operator ==(TInterface x, Serialized<TInterface, TSerialized> y) => y.Equals(x);
        public static bool operator !=(Serialized<TInterface, TSerialized> x, TInterface y) => !x.Equals(y);
        public static bool operator ==(Serialized<TInterface, TSerialized> x, TInterface y) => x.Equals(y);

        public readonly bool Equals(TInterface other)
        {
            if (other is TSerialized)
                return serializedObject == other;

            if (other is ISerializedInterface ySerialized)
                return serializedObject == ySerialized.SerializedObject;

            return false;
        }

        public readonly override bool Equals(object obj)
        {
            if (obj is ISerializedInterface ifaceSerialized)
                return serializedObject == ifaceSerialized.SerializedObject;

            if (obj is TInterface iface)
                return Equals(iface);

            return false;
        }

        public readonly override int GetHashCode() => serializedObject?.GetHashCode() ?? 0;

        void ISerializationCallbackReceiver.OnBeforeSerialize() => _value = null;
        void ISerializationCallbackReceiver.OnAfterDeserialize() => _value = null;
    }

    public static class InterfaceExtensions
    {
        public static SerializedInterface<T> AsSerialized<T>(this T interfaceObject) where T : class => new SerializedInterface<T>() { Value = interfaceObject };
    }
}
