#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Bipolar.Editor
{
	public static class UIToolkitHelper
	{
		public static void DrawProperty(SerializedProperty property, VisualElement container, System.Type requiredType, string label, ObjectCreationType buttons)
		{
			property.serializedObject.Update();
			container.style.flexDirection = FlexDirection.Row;
			container.style.marginRight = -2;

			var interfaceField = new InterfaceField(label, requiredType);
			interfaceField.BindProperty(property);
			container.Add(interfaceField);

			if (buttons != ObjectCreationType.None)
			{
				interfaceField.style.marginRight = 0;

				bool hasAddComponentButton = buttons.HasFlag(ObjectCreationType.AddComponent);
				bool hasCreateAssetButton = buttons.HasFlag(ObjectCreationType.CreateAsset);
				bool hasBothButtons = hasAddComponentButton && hasCreateAssetButton;

				container.style.alignItems = Align.Stretch;
				container.style.minWidth = 0;
				if (hasAddComponentButton)
				{
					var addComponentButton = new ObjectCreationButton("Add", ShowAddComponentDropDown);
					
					if (hasBothButtons)
					{
						addComponentButton.style.borderBottomRightRadius = 0;
						addComponentButton.style.borderTopRightRadius = 0;
					}

					container.Add(addComponentButton);

					void ShowAddComponentDropDown()
					{
						var dropdownRect = InterfaceEditorUtility.GetDropdownRect(container.worldBound);
						InterfaceEditorUtility.ShowAddComponentDropDown(requiredType, property, dropdownRect);
					}
				}

				if (hasCreateAssetButton)
				{
					var createAssetButton = new ObjectCreationButton("Create", ShowCreateAssetDropdown);

					if (hasBothButtons)
					{
						createAssetButton.style.marginLeft = -1;
						createAssetButton.style.borderBottomLeftRadius = 0;
						createAssetButton.style.borderTopLeftRadius = 0;
					}

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

	public class InterfaceField : ObjectField
	{
		public InterfaceField(string label, System.Type interfaceType) : base(label)
		{
			objectType = interfaceType;
			AddToClassList(alignedFieldUssClassName);
			this.Q(className: selectorUssClassName).style.display = DisplayStyle.None;

			style.flexGrow = 1;
			style.flexShrink = 1;
			style.minWidth = 0;

			var objectSelectorButton = new ObjectSelectorButton(this, interfaceType);
			this.Q(className: inputUssClassName).Add(objectSelectorButton);
		}
	}

	public class ObjectCreationButton : Button
	{
		public ObjectCreationButton(string text, System.Action onClick) : base(onClick)
		{
			this.text = text;
			style.flexShrink = 0.5f;
			style.marginLeft = 0;
			style.marginRight = 0;
		}
	}
}
#endif
