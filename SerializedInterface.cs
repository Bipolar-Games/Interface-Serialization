using System;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Bipolar
{
    [Serializable]
    public abstract class SerializedInterface<TInterface, TSerialized> : IEquatable<TInterface>, ISerializedInterface
        where TInterface : class
        where TSerialized : Object
    {
        [SerializeField]
        protected Serialized<TInterface, TSerialized> serializedValue;
        public TInterface Value
        {
            get => serializedValue.Value;
            set => serializedValue.Value = value;
        }

        public Type InterfaceType => typeof(TInterface);

        public override string ToString() => serializedValue.ToString();

        // Only one operand order is defined on purpose. Defining both (TInterface, SerializedInterface)
        // and (SerializedInterface, TInterface) makes `x == null` ambiguous for classes that derive
        // from SerializedInterface and implement TInterface at the same time.
        public static bool operator !=(SerializedInterface<TInterface, TSerialized> x, TInterface y) => !(x == y);
        public static bool operator ==(SerializedInterface<TInterface, TSerialized> x, TInterface y)
        {
            if (x is null)
                return y is null;

            if (y is null)
                return x.serializedValue.serializedObject == null;

            return x.Equals(y);
        }

        public bool Equals(TInterface other) => serializedValue.Equals(other);
        public override bool Equals(object obj) => serializedValue.Equals(obj);
        public override int GetHashCode() => serializedValue.GetHashCode();

        Object ISerializedInterface.SerializedObject => serializedValue.serializedObject;
    }

    [Serializable]
    public abstract class SerializedInterface<TInterface> : SerializedInterface<TInterface, Object>
        where TInterface : class
    { }
}
