using UnityEngine;

namespace Bipolar
{
    [System.Serializable]
    public class Serialized<TInterface> : Serialized<TInterface, Object>
        where TInterface : class
    { }
    
    [System.Serializable]
    public class Serialized<TInterface, TObject>
        where TInterface : class
        where TObject : Object
    {
        [SerializeField]
        private TObject serializedObject;

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
                if (!(value is TObject @object))
                    throw new System.InvalidCastException();

                serializedObject = @object;
                _value = value;
            }
        }

        public System.Type Type => typeof(TInterface);

        public static implicit operator TInterface(Serialized<TInterface, TObject> iface) => iface.Value;
    }
}
