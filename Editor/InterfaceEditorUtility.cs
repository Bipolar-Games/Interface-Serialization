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

        public static ObjectCreationType GetButtons(FieldInfo fieldInfo, ObjectCreationType @default = ObjectCreationType.None)
        {
            var buttonsAttribute = fieldInfo.GetCustomAttribute<NewObjectButtonAttribute>();

            var buttons = buttonsAttribute?.ButtonType ?? @default;
            return buttons;
        }

        public static string GetAssetFileName(System.Type type)
        {
            var attribute = type.GetCustomAttribute<CreateAssetMenuAttribute>();
            if (attribute != null)
            {
                string fileName = attribute.fileName;
                if (string.IsNullOrWhiteSpace(fileName) == false)
                    return fileName;
            }

            return "New " + ObjectNames.NicifyVariableName(type.Name);
        }
    }
}
