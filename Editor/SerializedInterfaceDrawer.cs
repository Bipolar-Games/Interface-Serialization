using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Bipolar.Editor
{
	[CustomPropertyDrawer(typeof(Serialized<>), true)]
	[CustomPropertyDrawer(typeof(Serialized<,>), true)]
	public class SerializedInterfaceDrawer : PropertyDrawer
	{
		private const string errorMessage = "Provided type is not an interface";

		private const string serializedObjectPropertyName = "serializedObject";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
			var requiredType = GetRequiredType();

			InterfaceEditorGUI.InterfaceField(position, label, serializedObjectProperty, requiredType, true);

			EditorGUI.EndProperty();
		}

		private System.Type GetRequiredType()
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
					if (type.GetGenericTypeDefinition() == typeof(Serialized<>))
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

	public static class InterfaceSelectorButton
	{
		public const float Width = 20;
	}
}
