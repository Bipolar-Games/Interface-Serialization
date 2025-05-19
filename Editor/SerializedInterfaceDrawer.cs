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

            var buttons = InterfaceEditorUtility.GetButtons(fieldInfo);

            bool hasAddComponentButton = buttons.HasFlag(InterfaceButtonType.AddComponent);
            bool hasCreateAssetButton = buttons.HasFlag(InterfaceButtonType.CreateAsset);
            bool hasBothButtons = hasCreateAssetButton && hasAddComponentButton;

            if (hasCreateAssetButton)
            {
                var buttonStyle = hasBothButtons ? EditorStyles.miniButtonRight : EditorStyles.miniButton;
                position = DrawSideButton(position, "Create", buttonStyle, 50);
            }

            if (hasAddComponentButton)
            {
                var buttonStyle = hasBothButtons ? EditorStyles.miniButtonLeft : EditorStyles.miniButton; 
                position = DrawSideButton(position, "Add", buttonStyle, 40);
            }

            var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
            var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);
            InterfaceEditorGUI.InterfaceField(position, label, serializedObjectProperty, requiredType);

            EditorGUI.EndProperty();
        }

        private Rect DrawSideButton(Rect position, string label, GUIStyle style, float width)
        {
            position.xMax -= width;
            var buttonRect = new Rect(position.xMax, position.y, width, position.height);
            if (GUI.Button(buttonRect, label, style))
            {
                Debug.Log($"{label} button clicked!");
            }
            return position;
        }
    }
}
