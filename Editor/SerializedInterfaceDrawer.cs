using UnityEditor;
using UnityEngine;
#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEngine.UIElements;
#endif

namespace Bipolar.Editor
{
	[CustomPropertyDrawer(typeof(Serialized<>), true)]
	[CustomPropertyDrawer(typeof(Serialized<,>), true)]
	public class SerializedInterfaceDrawer : PropertyDrawer
	{
		private const string serializedObjectPropertyName = "serializedObject";

#if !BIPOLAR_DISABLE_UI_TOOLKIT
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new VisualElement();

			var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
			var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);
			var buttons = InterfaceEditorUtility.GetButtons(fieldInfo);

			InterfaceEditorUIToolkitHelper.DrawProperty(serializedObjectProperty, container, requiredType, property.displayName, buttons);

			return container;
		}
#endif

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

            var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
            var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);
            var buttons = InterfaceEditorUtility.GetButtons(fieldInfo);
			
			InterfaceEditorGUI.DrawInterfaceProperty(position, serializedObjectProperty, label, requiredType, buttons);

			EditorGUI.EndProperty();
		}
	}
}
