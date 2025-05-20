using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bipolar.Editor
{
	public static class InterfaceEditorUtility
	{
        public static System.Type GetRequiredType(FieldInfo fieldInfo)
		{
			var type = fieldInfo.FieldType;
			while (type != null)
			{
				if (type.IsArray)
					type = type.GetElementType();

				if (type == null)
					return null;

				if (type.IsGenericType)
				{
					if (type.GetGenericTypeDefinition() == typeof(Serialized<,>))
						return type.GetGenericArguments()[0];

					if (typeof(IEnumerable).IsAssignableFrom(type))
					{
						type = type.GetGenericArguments()?[0];
						continue;
					}
				}

				type = type.BaseType;
			}

			return null;
		}

        public static InterfaceButtonType GetButtons(FieldInfo fieldInfo, InterfaceButtonType @default = InterfaceButtonType.None)
        {
            var buttonsAttribute = fieldInfo.GetCustomAttribute<InterfaceButtonAttribute>();

            var buttons = buttonsAttribute?.ButtonType ?? @default;
            return buttons;
        }

        public static string GetAssetFileName(System.Type type)
        {
            var attribute = type.GetCustomAttribute<CreateAssetMenuAttribute>();
            if (attribute != null)
            {
                string fileName = attribute.fileName;
                if (fileName != null)
                    return fileName;
            }

            return "New " + ObjectNames.NicifyVariableName(type.Name);
        }
    }
}
