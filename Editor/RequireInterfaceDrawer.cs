using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bipolar.Editor
{
	public class ObjectSelectorButton : VisualElement
	{
		public ObjectSelectorButton()
		{
			AddToClassList(ObjectField.selectorUssClassName);
		}
	}

	public class InterfaceField : BaseField<Object>
	{
		public InterfaceField(string label) : base(label, null)
		{
			var objectDisplay = new ObjectDisplay();
			var objectSelectorButton = new ObjectSelectorButton();
			AddToClassList(alignedFieldUssClassName);
			Add(objectDisplay);
			Add(objectSelectorButton);
		}
	}

	public class ObjectDisplay : VisualElement
	{
		private readonly Image objectIcon;
		private readonly Label objectLabel;

		private static readonly string ussClassName = "unity-object-field-display";

		private static readonly string iconUssClassName = ussClassName + "__icon";

		internal static readonly string labelUssClassName = ussClassName + "__label";

		private static readonly string acceptDropVariantUssClassName = ussClassName + "--accept-drop";

		public ObjectDisplay()
		{
			AddToClassList(ussClassName);
			objectIcon = new Image
			{
				scaleMode = ScaleMode.ScaleAndCrop,
				pickingMode = PickingMode.Ignore
			};
			objectIcon.AddToClassList(iconUssClassName);
			objectLabel = new Label
			{
				pickingMode = PickingMode.Ignore
			};
			objectLabel.AddToClassList(labelUssClassName);
			//m_ObjectField = objectField;
			//Update();
			Add(objectIcon);
			Add(objectLabel);
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
					value = property.objectReferenceValue
				};
				objectField.AddToClassList(ObjectField.alignedFieldUssClassName);

				objectField.RegisterValueChangedCallback(evt =>
				{
					property.objectReferenceValue = evt.newValue;
					property.serializedObject.ApplyModifiedProperties();
				});

				var objectSelectorButton = new ObjectSelectorButton();


				//container.Add(objectField);
				//container.Add(objectSelectorButton);

				var toggle= new Toggle("Show Object Selector Button");
			
				var interfaceField = new InterfaceField(property.displayName);
				container.Add(interfaceField);

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
