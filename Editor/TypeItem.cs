using System;
using UnityEditor.IMGUI.Controls;
using UnityEditor;

namespace Bipolar.Editor
{
    internal class TypeItem : AdvancedDropdownItem
    {
        public Type Type { get; }

        public TypeItem(Type type) : this(type, ObjectNames.NicifyVariableName(type.Name))
        { }

        public TypeItem(Type type, string name) : base(name)
        {
            Type = type;
        }
    }
}
