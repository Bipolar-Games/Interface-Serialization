using UnityEditor;
using UnityEngine;
#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEngine.UIElements;
#endif

namespace Bipolar.Editor
{
	[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
	public class RequireInterfaceDrawer : PropertyDrawer
	{
		private const string errorMessageText = "Property is not a reference type";
		private static readonly GUIContent errorMessage = new GUIContent(errorMessageText);

#if !BIPOLAR_DISABLE_UI_TOOLKIT
		private static readonly Label errorLabel = new Label(errorMessageText);

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new VisualElement();
			DrawProperty();
			return container;

			void DrawProperty()
			{
				if (property.propertyType != SerializedPropertyType.ObjectReference)
				{
					container.Add(errorLabel);
					container.style.color = Color.red;
					return;
				}

				var requiredAttribute = attribute as RequireInterfaceAttribute;
				var requiredType = requiredAttribute.RequiredType;
				var buttons = InterfaceEditorUtility.GetButtons(fieldInfo, requiredAttribute.ButtonType);

				UIToolkitHelper.DrawProperty(property, container, requiredType, property.displayName, buttons);
			}
		}
#endif

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference)
			{
				var previousColor = GUI.color;
				GUI.color = Color.red;
				EditorGUI.LabelField(position, label, errorMessage);
				GUI.color = previousColor;
				return;
			}

			EditorGUI.BeginProperty(position, label, property);

			var requiredAttribute = attribute as RequireInterfaceAttribute;
			var requiredType = requiredAttribute.RequiredType;
			var buttons = InterfaceEditorUtility.GetButtons(fieldInfo, requiredAttribute.ButtonType);

			InterfaceEditorGUI.DrawInterfaceProperty(position, property, label, requiredType, buttons);

			EditorGUI.EndProperty();
		}
	}
}
