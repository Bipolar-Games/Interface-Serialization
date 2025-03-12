using System.Collections;
using System.Reflection;

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
	}
}
