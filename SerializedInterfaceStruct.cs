using System;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Bipolar
{
    internal interface IObjectContainter
    {
        Object SerializedObject { get; }
    }

    [Serializable]
    public struct Serialized<TInterface> : ISerializedInterface<TInterface>, IEquatable<TInterface>, IObjectContainter
        where TInterface : class
    {
        [SerializeField]
        internal Serialized<TInterface, Object> serializedValue;
        public TInterface Value
        {
            readonly get => serializedValue.Value;
            set => serializedValue.Value = value;
        }

        public readonly Type InterfaceType => typeof(TInterface);

        public static bool operator !=(TInterface x, Serialized<TInterface> y) => !y.Equals(x);
        public static bool operator ==(TInterface x, Serialized<TInterface> y) => y.Equals(x);
        public static bool operator !=(Serialized<TInterface> x, TInterface y) => !x.Equals(y);
        public static bool operator ==(Serialized<TInterface> x, TInterface y) => x.Equals(y);

        public static bool operator !=(object x, Serialized<TInterface> y) => !y.Equals(x);
        public static bool operator ==(object x, Serialized<TInterface> y) => y.Equals(x);
        public static bool operator !=(Serialized<TInterface> x, object y) => !x.Equals(y);
        public static bool operator ==(Serialized<TInterface> x, object y) => x.Equals(y);

        public readonly bool Equals(TInterface other) => serializedValue.Equals(other);
        public override readonly bool Equals(object obj) => serializedValue.Equals(obj);
        public override readonly int GetHashCode() => serializedValue.GetHashCode();

        readonly Object IObjectContainter.SerializedObject => serializedValue.serializedObject;
    }

    [Serializable]
    public struct Serialized<TInterface, TSerialized> : ISerializedInterface<TInterface>, IEquatable<TInterface>, ISerializationCallbackReceiver, IObjectContainter
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

        public override string ToString() => Value?.ToString() ?? "null";

        public static bool operator !=(TInterface x, Serialized<TInterface, TSerialized> y) => !y.Equals(x);
        public static bool operator ==(TInterface x, Serialized<TInterface, TSerialized> y) => y.Equals(x);
        public static bool operator !=(Serialized<TInterface, TSerialized> x, TInterface y) => !x.Equals(y);
        public static bool operator ==(Serialized<TInterface, TSerialized> x, TInterface y) => x.Equals(y);

        public readonly bool Equals(TInterface other)
        {
            if (other is TSerialized)
                return serializedObject == other;

            if (other is IObjectContainter ySerialized)
                return serializedObject == ySerialized.SerializedObject;

            return false;
        }

        public readonly override bool Equals(object obj)
        {
            if (obj is IObjectContainter ifaceSerialized)
                return serializedObject == ifaceSerialized.SerializedObject;

            if (obj is TInterface iface)
                return Equals(iface);

            return false;
        }

        public readonly override int GetHashCode() => serializedObject?.GetHashCode() ?? 0;

        void ISerializationCallbackReceiver.OnBeforeSerialize() => _value = null;
        void ISerializationCallbackReceiver.OnAfterDeserialize() => _value = null;
        readonly Object IObjectContainter.SerializedObject => serializedObject;
    }
}
