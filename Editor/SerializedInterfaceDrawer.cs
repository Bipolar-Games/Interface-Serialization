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
			UIToolkitHelper.DrawProperty(serializedObjectProperty, container, requiredType);

			return container;
		}
#endif
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
            var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);

            var buttons = InterfaceEditorUtility.GetButtons(fieldInfo);
            bool hasAddComponentButton = buttons.HasFlag(ObjectCreationType.AddComponent);
            bool hasCreateAssetButton = buttons.HasFlag(ObjectCreationType.CreateAsset);
            bool hasBothButtons = hasCreateAssetButton && hasAddComponentButton;

            var originalRect = position;
            if (hasCreateAssetButton)
            {
                var buttonStyle = hasBothButtons ? EditorStyles.miniButtonRight : EditorStyles.miniButton;
                position = InterfaceEditorGUI.DrawCreateAssetButton(position, serializedObjectProperty, buttonStyle, requiredType, originalRect);
            }

            if (hasAddComponentButton)
            {
                var buttonStyle = hasBothButtons ? EditorStyles.miniButtonLeft : EditorStyles.miniButton;
                position = InterfaceEditorGUI.DrawAddComponentButton(position, serializedObjectProperty, buttonStyle, requiredType, originalRect);
            }

            InterfaceEditorGUI.InterfaceField(position, label, serializedObjectProperty, requiredType);

            EditorGUI.EndProperty();
        }
    }
}
