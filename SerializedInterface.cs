using JetBrains.Annotations;
using UnityEngine;

namespace Bipolar
{
    [System.Serializable]
    public class Serialized<TInterface> : Serialized<TInterface, Object>
        where TInterface : class
    {
        public static explicit operator Serialized<TInterface>(TInterface iface) => new Serialized<TInterface>() { Value = iface };

        public static bool operator ==(Serialized<TInterface> x, TInterface y) => x.Value == y;
        public static bool operator ==(TInterface x, Serialized<TInterface> y) => x == y.Value;
        public static bool operator !=(Serialized<TInterface> x, TInterface y) => x.Value != y;
        public static bool operator !=(TInterface x, Serialized<TInterface> y) => x != y.Value;
    }

    [System.Serializable]
    public class Serialized<TInterface, TSerialized>
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
                if (value is TSerialized @object)
                {
                    serializedObject = @object;
                    _value = value;
                }
                else
                {
                    throw new System.InvalidCastException();
                }
            }
        }

        public System.Type Type => typeof(TInterface);

        public static implicit operator TInterface(Serialized<TInterface, TSerialized> iface) => iface.Value;
        public static explicit operator Serialized<TInterface, TSerialized>(TInterface iface) => new Serialized<TInterface, TSerialized>() { Value = iface };

        public override string ToString() => Value.ToString();
    }

    public static class InterfaceExtensions
    {
        public static Serialized<T> AsSerialized<T>(this T interfaceObject) where T : class => new Serialized<T>() { Value = interfaceObject };
    }
}
