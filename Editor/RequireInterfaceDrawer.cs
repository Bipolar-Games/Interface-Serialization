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

				container.style.flexDirection = FlexDirection.Row;

				var requiredAttribute = attribute as RequireInterfaceAttribute;
				var requiredType = requiredAttribute.RequiredType;
				UIToolkitHelper.DrawProperty(property, container, requiredType, property.displayName);

				var buttons = InterfaceEditorUtility.GetButtons(fieldInfo, requiredAttribute.ButtonType);
				if (buttons != ObjectCreationType.None)
				{
					bool hasAddComponentButton = buttons.HasFlag(ObjectCreationType.AddComponent);
					bool hasCreateAssetButton = buttons.HasFlag(ObjectCreationType.CreateAsset);
					bool hasBothButtons = hasAddComponentButton && hasCreateAssetButton;

					container.style.alignItems = Align.Stretch;
					container.style.minWidth = 0;

					if (hasAddComponentButton)
					{
						var addComponentButton = new Button(ShowAddComponentDropDown)
						{
							text = "Add",
							style =
							{
								marginLeft = 2,
								marginRight = 0,
								flexShrink = 0.5f,
							}
						};
						container.Add(addComponentButton);

						void ShowAddComponentDropDown()
						{
							var dropdownRect = InterfaceEditorUtility.GetDropdownRect(container.worldBound);
							InterfaceEditorUtility.ShowAddComponentDropDown(requiredType, property, dropdownRect);
						}
					}

					if (hasCreateAssetButton)
					{
						var createAssetButton = new Button(ShowCreateAssetDropdown)
						{
							text = "Create",
							style =
							{
								marginLeft = 0,
								marginRight = 0,
								flexShrink = 0.5f,
							}
						};
						container.Add(createAssetButton);

						void ShowCreateAssetDropdown()
						{
							var dropdownRect = InterfaceEditorUtility.GetDropdownRect(container.worldBound);
							InterfaceEditorUtility.ShowCreateAssetDropdown(requiredType, property, dropdownRect);
						}

					}
				}

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

				var buttons = InterfaceEditorUtility.GetButtons(fieldInfo, requiredAttribute.ButtonType);
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
