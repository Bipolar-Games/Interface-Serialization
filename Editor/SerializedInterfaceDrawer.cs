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

			var objectFieldRect = new Rect(position.x, position.y, position.width - InterfaceSelectorButton.Width, position.height);
			var interfaceButtonRect = new Rect(position.x + objectFieldRect.width, position.y, InterfaceSelectorButton.Width, position.height);

			var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);

			var requiredType = GetRequiredType();
			if (requiredType != default)
			{
				serializedObjectProperty.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, label, serializedObjectProperty.objectReferenceValue, requiredType, true);
				if (GUI.Button(interfaceButtonRect, "I"))
				{
					Object objectReferenceValue = serializedObjectProperty.objectReferenceValue;
					InterfaceSelectorWindow.Show(requiredType, objectReferenceValue, (obj) => AssignValue(serializedObjectProperty, obj));
				}
			}
			else
			{

			}


			EditorGUI.EndProperty();
		}

		private static void AssignValue(SerializedProperty property, Object @object)
		{
			property.objectReferenceValue = @object;
			property.serializedObject.ApplyModifiedProperties();
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
