using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bipolar.Editor
{
	public class ObjectSelectorButton : VisualElement
	{
		private BaseField<Object> objectField;
		private System.Type requiredType;

		public ObjectSelectorButton(BaseField<Object> objectField, System.Type requiredType)
		{
			this.objectField = objectField;
			this.requiredType = requiredType;
			AddToClassList(ObjectField.selectorUssClassName);
		}

		[EventInterest(new System.Type[] { typeof(MouseDownEvent) })]
		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (evt is MouseDownEvent mouseDownEvent && mouseDownEvent.button == 0)
			{
				InterfaceSelectorWindow.Show(requiredType, objectField.value, AssignValue);
			}
		}

		private void AssignValue(Object selected)
		{
			objectField.value = selected;
		}
	}

	[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
	public class RequireInterfaceDrawer : PropertyDrawer
	{
		private const string errorMessageText = "Property is not a reference type";
		private static readonly Label errorLabel = new Label(errorMessageText);
		private static readonly GUIContent errorMessage = new GUIContent(errorMessageText);

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new VisualElement();
			DrawProperty(property, container);
			return container;

			void DrawProperty(SerializedProperty property, VisualElement container)
			{
				property.serializedObject.Update();
				if (property.propertyType != SerializedPropertyType.ObjectReference)
				{
					container.Add(errorLabel);
					container.style.color = Color.red;
					return;
				}

				var requiredAttribute = attribute as RequireInterfaceAttribute;
				var requiredType = requiredAttribute.RequiredType;

				var objectField = new ObjectField(property.displayName)
				{
					objectType = requiredType,
				};
				objectField.BindProperty(property);
				objectField.AddToClassList(ObjectField.alignedFieldUssClassName);
				objectField.Q(className: ObjectField.selectorUssClassName).style.display = DisplayStyle.None;

				var objectSelectorButton = new ObjectSelectorButton(objectField, requiredType);
				objectField.Q(className: ObjectField.inputUssClassName).Add(objectSelectorButton);

				container.Add(objectField);
			}
		}


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
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
					position = InterfaceEditorGUI.DrawCreateAssetButton(position, property, buttonStyle, requiredType);
				}

				if (hasAddComponentButton)
				{
					var buttonStyle = hasBothButtons ? EditorStyles.miniButtonLeft : EditorStyles.miniButton;
					position = InterfaceEditorGUI.DrawAddComponentButton(position, property, buttonStyle, requiredType);
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
