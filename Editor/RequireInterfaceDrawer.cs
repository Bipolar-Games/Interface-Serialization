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
				UIToolkitHelper.DrawProperty(property, container, requiredAttribute.RequiredType);
			}
		}
#endif

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var originalPosition = position;
			if (property.propertyType == SerializedPropertyType.ObjectReference)
			{
				EditorGUI.BeginProperty(position, label, property);

				var requiredAttribute = attribute as RequireInterfaceAttribute;
				var requiredType = requiredAttribute.RequiredType;

				var buttons = requiredAttribute.ButtonType;
				buttons = InterfaceEditorUtility.GetButtons(fieldInfo, buttons);
				bool hasAddComponentButton = buttons.HasFlag(ObjectCreationType.AddComponent);
				bool hasCreateAssetButton = buttons.HasFlag(ObjectCreationType.CreateAsset);
				bool hasBothButtons = hasCreateAssetButton && hasAddComponentButton;

				if (hasCreateAssetButton)
				{
					var buttonStyle = hasBothButtons ? EditorStyles.miniButtonRight : EditorStyles.miniButton;
					position = InterfaceEditorGUI.DrawCreateAssetButton(position, property, buttonStyle, requiredType, originalPosition);
				}

				if (hasAddComponentButton)
				{
					var buttonStyle = hasBothButtons ? EditorStyles.miniButtonLeft : EditorStyles.miniButton;
					position = InterfaceEditorGUI.DrawAddComponentButton(position, property, buttonStyle, requiredType, originalPosition);
				}

				InterfaceEditorGUI.InterfaceField(position, label, property, requiredType);

				EditorGUI.EndProperty();
			}
			else
			{
				var previousColor = GUI.color;
				GUI.color = Color.red;
				EditorGUI.LabelField(position, label, errorMessage);
				GUI.color = previousColor;
			}
		}
	}
}
