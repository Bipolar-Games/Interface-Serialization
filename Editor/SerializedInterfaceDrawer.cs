using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEngine.UIElements;
#endif

namespace Bipolar.Editor
{
    [CustomPropertyDrawer(typeof(Serialized<>))]
    [CustomPropertyDrawer(typeof(SerializedInterface<>), true)]
	[CustomPropertyDrawer(typeof(SerializedInterface<,>), true)]
	public class SerializedInterfaceDrawer : PropertyDrawer
	{
        private const string serializedValuePropertyName = "serializedValue";

#if !BIPOLAR_DISABLE_UI_TOOLKIT
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var internalProperty = property.FindPropertyRelative(serializedValuePropertyName);
            if (internalProperty == null)
            {
                var missing = new Label($"Missing {serializedValuePropertyName}");
                return missing;
            }

            return SerializedInterfaceStructDrawer.CreatePropertyGUI(internalProperty, fieldInfo, property.displayName);
        }
#endif

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var internalProperty = property.FindPropertyRelative(serializedValuePropertyName);
			return EditorGUI.GetPropertyHeight(internalProperty, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            EditorGUI.BeginProperty(position, label, property);
			var internalProperty = property.FindPropertyRelative(serializedValuePropertyName);
			EditorGUI.PropertyField(position, internalProperty, label);
            EditorGUI.EndProperty();
        }
    }
}
