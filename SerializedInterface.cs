using System;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Bipolar
{
    [Serializable]
    public abstract class SerializedInterface<TInterface, TSerialized> : ISerializedInterface, ISerializationCallbackReceiver, IEquatable<TInterface>
        where TInterface : class
        where TSerialized : Object
    {
        [SerializeField]
        protected Serialized<TInterface, TSerialized> serializedValue;
        public TInterface Value => serializedValue.Value; 

        public Type InterfaceType => typeof(TInterface);

        Object ISerializedInterface.SerializedObject => serializedValue.serializedObject;

        public override string ToString() => serializedValue.ToString();

        public static bool operator !=(TInterface x, SerializedInterface<TInterface, TSerialized> y) => !y.Equals(x);
        public static bool operator ==(TInterface x, SerializedInterface<TInterface, TSerialized> y) => y.Equals(x);
        public static bool operator !=(SerializedInterface<TInterface, TSerialized> x, TInterface y) => !x.Equals(y);
        public static bool operator ==(SerializedInterface<TInterface, TSerialized> x, TInterface y) => x.Equals(y);

        public bool Equals(TInterface other) => serializedValue.Equals(other);

        public override bool Equals(object obj) => serializedValue.Equals(obj);

        public override int GetHashCode() => serializedValue.GetHashCode();

        void ISerializationCallbackReceiver.OnBeforeSerialize() => serializedValue.Value = null;
        void ISerializationCallbackReceiver.OnAfterDeserialize() => serializedValue.Value = null;
    }

    [Serializable]
    public abstract class SerializedInterface<TInterface> : SerializedInterface<TInterface, Object>
        where TInterface : class
    { }
}
