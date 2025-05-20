using UnityEngine;
using UnityEditor;

namespace Bipolar.Editor
{
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    public class RequireInterfaceDrawer : PropertyDrawer
    {
        private const string errorMessage = "Property is not a reference type";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                EditorGUI.BeginProperty(position, label, property);

                var requiredAttribute = attribute as RequireInterfaceAttribute;
                var requiredType = requiredAttribute.RequiredType;

                var buttons = InterfaceEditorUtility.GetButtons(fieldInfo);
                bool hasAddComponentButton = buttons.HasFlag(InterfaceButtonType.AddComponent);
                bool hasCreateAssetButton = buttons.HasFlag(InterfaceButtonType.CreateAsset);
                bool hasBothButtons = hasCreateAssetButton && hasAddComponentButton;

                if (hasCreateAssetButton)
                {
                    var buttonStyle = hasBothButtons ? EditorStyles.miniButtonRight : EditorStyles.miniButton;
                    position = InterfaceEditorGUI.DrawCreateAssetButton(position, buttonStyle, requiredType);
                }

                if (hasAddComponentButton)
                {
                    var buttonStyle = hasBothButtons ? EditorStyles.miniButtonLeft : EditorStyles.miniButton;
                    position = InterfaceEditorGUI.DrawAddComponentButton(position, buttonStyle, requiredType);
                }

                InterfaceEditorGUI.InterfaceField(position, label, property, requiredType);

                EditorGUI.EndProperty();
            }
            else
            {
                var previousColor = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(position, label, new GUIContent(errorMessage));
                GUI.color = previousColor;
            }
        }
    }
}
