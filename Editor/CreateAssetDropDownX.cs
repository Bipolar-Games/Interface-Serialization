using System;
using UnityEngine;

namespace Bipolar.Editor
{
    internal class CreateAssetDropDownX : AAA
    {
        public CreateAssetDropDownX(Type requiredType) : base(requiredType)
        { }
     
        protected override string GetRootItemName() => "ScriptableObject";

        protected override void AddTypeToBuilder(TypeItemBuilder builder, Type type)
        {
            if (type.IsDefined(typeof(CreateAssetMenuAttribute), true))
            {
                var attribute = (CreateAssetMenuAttribute)type.GetCustomAttributes(typeof(CreateAssetMenuAttribute), true)[0];
                var path = attribute.menuName;
                string[] pathItems = path.Contains('/')
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
