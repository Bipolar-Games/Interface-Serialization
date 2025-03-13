using UnityEditor;
using UnityEngine;

namespace Bipolar.Editor
{
	[CustomPropertyDrawer(typeof(Serialized<>), true)]
	[CustomPropertyDrawer(typeof(Serialized<,>), true)]
	public class SerializedInterfaceDrawer : PropertyDrawer
	{
		private const string serializedObjectPropertyName = "serializedObject";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
			var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);
			InterfaceEditorGUI.InterfaceField(position, label, serializedObjectProperty, requiredType);

			EditorGUI.EndProperty();
		}
	}
}
