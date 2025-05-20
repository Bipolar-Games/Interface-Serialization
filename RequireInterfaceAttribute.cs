using UnityEngine;

namespace Bipolar
{
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public System.Type RequiredType { get; private set; }

        public InterfaceButtonType ButtonType { get; private set; }

        public RequireInterfaceAttribute(System.Type type, InterfaceButtonType buttonType = InterfaceButtonType.None)
        {
            RequiredType = type;
            ButtonType = buttonType;
        }
    }
}
