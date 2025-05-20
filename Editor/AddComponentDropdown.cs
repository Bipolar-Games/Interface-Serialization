using System;
using UnityEngine;

namespace Bipolar.Editor
{
    internal class AddComponentDropdown : TypeItemDropdown
    {
        public AddComponentDropdown(Type requiredType) : base(requiredType, typeof(Component))
        { }

        protected override string GetRootItemName() => "Component";

        protected override void AddTypeToBuilder(TypeItemBuilder builder, Type type)
        {
            if (type.IsDefined(typeof(AddComponentMenu), true))
            {
                var attribute = (AddComponentMenu)type.GetCustomAttributes(typeof(AddComponentMenu), true)[0];
                var path = attribute.componentMenu;
                string[] pathItems = path.Contains('/')
                    ? path.Split('/')
                    : path.Split('\\');

                var componentName = pathItems[pathItems.Length - 1];
                if (string.IsNullOrWhiteSpace(componentName) == false)
                {
                    builder.AddType(type, pathItems, attribute.componentOrder);
                    return;
                }
            }
            builder.AddType(type);
        }
    }
}
