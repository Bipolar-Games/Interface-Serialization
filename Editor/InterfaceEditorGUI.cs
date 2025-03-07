using UnityEditor;
using UnityEngine;

namespace Bipolar.Editor
{
	public static class InterfaceEditorGUI
	{
		public const float InterfaceSelectorButtonWidth = 20;
		
		public static Object InterfaceField(Rect rect, GUIContent label, Object obj, System.Type interfaceType, bool allowSceneObjects)
		{
			return default;
		}

		public static void InterfaceField(Rect rect, GUIContent label, SerializedProperty serializedObjectProperty, System.Type interfaceType, bool allowSceneObjects)
		{
			if (interfaceType == default)
				return;

			var objectFieldRect = new Rect(rect.x, rect.y, rect.width - InterfaceSelectorButtonWidth, rect.height);
			var interfaceButtonRect = new Rect(rect.x + objectFieldRect.width, rect.y, InterfaceSelectorButtonWidth, rect.height);

			serializedObjectProperty.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, label, serializedObjectProperty.objectReferenceValue, interfaceType, allowSceneObjects);
			if (GUI.Button(interfaceButtonRect, "I"))
			{
				Object objectReferenceValue = serializedObjectProperty.objectReferenceValue;
				InterfaceSelectorWindow.Show(interfaceType, objectReferenceValue, (obj) => AssignValue(serializedObjectProperty, obj));
			}

			static void AssignValue(SerializedProperty property, Object @object)
			{
				property.objectReferenceValue = @object;
				property.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
