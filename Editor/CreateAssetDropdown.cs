using System;
using UnityEngine;

namespace Bipolar.Editor
{
    internal class CreateAssetDropdown : TypeItemDropdown
    {
        public CreateAssetDropdown(Type requiredType) : base(requiredType, typeof(ScriptableObject))
        { }
     
        protected override string GetRootItemName() => "ScriptableObject";

        protected override void AddTypeToBuilder(TypeItemBuilder builder, Type type)
        {
            if (type.IsDefined(typeof(CreateAssetMenuAttribute), true))
            {
                var attribute = (CreateAssetMenuAttribute)type.GetCustomAttributes(typeof(CreateAssetMenuAttribute), true)[0];
                var path = attribute.menuName;
                string[] pathItems = path.IndexOf('/') >= 0
                    ? path.Split('/')
                    : path.Split('\\');
                var componentName = pathItems[pathItems.Length - 1];
                if (string.IsNullOrWhiteSpace(componentName) == false)
                {
                    builder.AddType(type, pathItems, attribute.order);
                    return;
                }
            }
            builder.AddType(type);
        }
    }
}
