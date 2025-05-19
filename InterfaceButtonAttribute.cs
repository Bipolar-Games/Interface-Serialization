using UnityEngine;

namespace Bipolar
{
    public class InterfaceButtonAttribute : PropertyAttribute
    {
        public InterfaceButtonType ButtonType { get; private set; }

        public InterfaceButtonAttribute(InterfaceButtonType buttonType)
        {
            ButtonType = buttonType;
        }
    }

    public enum InterfaceButtonType
    {
        None = 0,
        AddComponent = 1 << 0,
        CreateAsset = 1 << 1,
        Both = AddComponent | CreateAsset
    }
}
