using UnityEngine;

namespace Bipolar
{
    public class NewObjectButtonAttribute : PropertyAttribute
    {
        public ObjectCreationType ButtonType { get; private set; }

        public NewObjectButtonAttribute(ObjectCreationType buttonType)
        {
            ButtonType = buttonType;
        }
    }

    [System.Flags]
    public enum ObjectCreationType
    {
        None = 0,
        AddComponent = 1 << 0,
        CreateAsset = 1 << 1,
        Both = AddComponent | CreateAsset
    }
}
