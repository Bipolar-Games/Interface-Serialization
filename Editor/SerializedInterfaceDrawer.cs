using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bipolar.Editor
{
	[CustomPropertyDrawer(typeof(Serialized<>), true)]
    [CustomPropertyDrawer(typeof(Serialized<,>), true)]
    public class SerializedInterfaceDrawer : PropertyDrawer
    {
        private const string serializedObjectPropertyName = "serializedObject";
        
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
            var container = new VisualElement();

			var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
			var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);
			UIToolkitHelper.DrawProperty(serializedObjectProperty, container, requiredType);

			return container;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
            var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);

            var buttons = InterfaceEditorUtility.GetButtons(fieldInfo);
            bool hasAddComponentButton = buttons.HasFlag(ObjectCreationType.AddComponent);
            bool hasCreateAssetButton = buttons.HasFlag(ObjectCreationType.CreateAsset);
            bool hasBothButtons = hasCreateAssetButton && hasAddComponentButton;

            if (hasCreateAssetButton)
            {
                var buttonStyle = hasBothButtons ? EditorStyles.miniButtonRight : EditorStyles.miniButton;
                position = InterfaceEditorGUI.DrawCreateAssetButton(position, serializedObjectProperty, buttonStyle, requiredType);
            }

            if (hasAddComponentButton)
            {
                var buttonStyle = hasBothButtons ? EditorStyles.miniButtonLeft : EditorStyles.miniButton;
                position = InterfaceEditorGUI.DrawAddComponentButton(position, serializedObjectProperty, buttonStyle, requiredType);
            }

            InterfaceEditorGUI.InterfaceField(position, label, serializedObjectProperty, requiredType);

            EditorGUI.EndProperty();
        }
    }
}
