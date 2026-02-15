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
			container.style.flexDirection = FlexDirection.Row;

			var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
			var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);

			UIToolkitHelper.DrawProperty(serializedObjectProperty, container, requiredType, property.displayName);

			var buttons = InterfaceEditorUtility.GetButtons(fieldInfo);
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
						InterfaceEditorUtility.ShowAddComponentDropDown(requiredType, serializedObjectProperty, dropdownRect);
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
						InterfaceEditorUtility.ShowCreateAssetDropdown(requiredType, serializedObjectProperty, dropdownRect);
					}

				}
			}

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
