using UnityEngine;

namespace Bipolar
{
    public class NewObjectButtonAttribute : PropertyAttribute
    {
        public ObjectCreationTypes ButtonType { get; private set; }

        public NewObjectButtonAttribute(ObjectCreationTypes buttonType)
        {
            ButtonType = buttonType;
        }
    }

    [System.Flags]
    public enum ObjectCreationTypes
    {
        None = 0,
        AddComponent = 1 << 0,
        CreateAsset = 1 << 1,
        Both = AddComponent | CreateAsset
    }
}
