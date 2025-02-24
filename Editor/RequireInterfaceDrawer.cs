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
                var objectFieldRect = new Rect(position.x, position.y, position.width - InterfaceSelectorButton.Width, position.height);
                var interfaceButtonRect = new Rect(position.x + objectFieldRect.width, position.y, InterfaceSelectorButton.Width, position.height);
                
                var requiredAttribute = attribute as RequireInterfaceAttribute;
                EditorGUI.BeginProperty(position, label, property);
                var requiredType = requiredAttribute.RequiredType;
                property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, label, property.objectReferenceValue, requiredType, true);
                if (GUI.Button(interfaceButtonRect, "I"))
                {
                    InterfaceSelectorWindow.Show(requiredType, property.objectReferenceValue, (obj) => AssignValue(property, obj));
                }

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

        private static void AssignValue (SerializedProperty interfaceProperty, Object @object)
        {
            interfaceProperty.objectReferenceValue = @object;
            interfaceProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
