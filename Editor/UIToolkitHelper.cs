#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Bipolar.Editor
{
	public static class UIToolkitHelper
	{
		public static void DrawProperty(SerializedProperty property, VisualElement container, System.Type requiredType)
		{
			property.serializedObject.Update();
			var objectField = new ObjectField(property.displayName)
			{
				objectType = requiredType,
			};
			objectField.BindProperty(property);
			objectField.AddToClassList(ObjectField.alignedFieldUssClassName);
			objectField.Q(className: ObjectField.selectorUssClassName).style.display = DisplayStyle.None;

			objectField.style.flexGrow = 1;
			objectField.style.flexShrink = 1;
			objectField.style.minWidth = 0;

			var objectSelectorButton = new ObjectSelectorButton(objectField, requiredType);
			objectField.Q(className: ObjectField.inputUssClassName).Add(objectSelectorButton);

			container.Add(objectField);

		}
	}
}
#endif
