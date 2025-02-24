using UnityEngine;

namespace Bipolar
{
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public System.Type RequiredType { get; private set; }

        public RequireInterfaceAttribute(System.Type type)
        {
            RequiredType = type;
        }
    }
}
