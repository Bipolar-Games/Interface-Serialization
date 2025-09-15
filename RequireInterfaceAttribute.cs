using UnityEngine;

namespace Bipolar
{
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public System.Type RequiredType { get; private set; }

        public ObjectCreationType ButtonType { get; private set; }

        public RequireInterfaceAttribute(System.Type type, ObjectCreationType buttonType = ObjectCreationType.None)
        {
            RequiredType = type;
            ButtonType = buttonType;
        }
    }
}
