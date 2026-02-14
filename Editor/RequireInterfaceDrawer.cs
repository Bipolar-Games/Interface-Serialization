using Codice.CM.Common;
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
		private bool allowSceneObjects;
		private System.Type m_objectType;
		private ObjectDisplay m_ObjectFieldDisplay;

		public InterfaceField(string label) : base(label, null)
		{
			//base.visualInput.focusable = false;
			labelElement.focusable = false;
			AddToClassList(ObjectField.ussClassName);
			labelElement.AddToClassList(ObjectField.labelUssClassName);
			allowSceneObjects = true;
			m_objectType = typeof(Object);
			m_ObjectFieldDisplay = new ObjectDisplay(this)
			{
				focusable = true
			};
			m_ObjectFieldDisplay.AddToClassList(ObjectField.objectUssClassName);
			var objectSelectorButton = new ObjectSelectorButton();
			objectSelectorButton.AddToClassList(ObjectField.selectorUssClassName);


			AddToClassList(alignedFieldUssClassName);
			Add(m_ObjectFieldDisplay);
			Add(objectSelectorButton);
		}
	}


	public class MyObjectField : VisualElement
	{
		private Object _value;
		private System.Type _objectType;

		private Label _label;
		private VisualElement _objectDisplay;

		public Object value
		{
			get => _value;
			set
			{
				if (_value == value) return;
				_value = value;
				UpdateDisplay();
			}
		}

		public System.Type objectType
		{
			get => _objectType;
			set => _objectType = value;
		}

		public MyObjectField(System.Type type)
		{
			_objectType = type;

			style.flexDirection = FlexDirection.Row;
			style.alignItems = Align.Center;

			_objectDisplay = new VisualElement();
			_objectDisplay.style.flexGrow = 1;
			_objectDisplay.style.height = 18;
			_objectDisplay.style.borderBottomWidth = 1;
			_objectDisplay.style.borderTopWidth = 1;
			_objectDisplay.style.borderLeftWidth = 1;
			_objectDisplay.style.borderRightWidth = 1;
			_objectDisplay.style.paddingLeft = 4;

			_label = new Label();
			_objectDisplay.Add(_label);

			Add(_objectDisplay);

			RegisterCallbacks();
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			if (_value == null)
			{
				_label.text = "None";
				_objectDisplay.style.backgroundImage = null;
			}
			else
			{
				_label.text = _value.name;

				var icon = EditorGUIUtility.ObjectContent(_value, _value.GetType()).image;
				_objectDisplay.style.backgroundImage = new StyleBackground((Texture2D)icon);
			}
		}

		private void RegisterCallbacks()
		{
			_objectDisplay.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
			_objectDisplay.RegisterCallback<DragPerformEvent>(OnDragPerform);
			_objectDisplay.RegisterCallback<MouseDownEvent>(OnMouseDown);
		}

		private void OnDragUpdated(DragUpdatedEvent evt)
		{
			if (DragAndDrop.objectReferences.Length > 0 &&
				_objectType.IsAssignableFrom(DragAndDrop.objectReferences[0].GetType()))
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			}
		}

		private void OnDragPerform(DragPerformEvent evt)
		{
			if (DragAndDrop.objectReferences.Length > 0 &&
				_objectType.IsAssignableFrom(DragAndDrop.objectReferences[0].GetType()))
			{
				value = DragAndDrop.objectReferences[0];
				DragAndDrop.AcceptDrag();
			}
		}

		private int _pickerControlID;

		private void OnMouseDown(MouseDownEvent evt)
		{
			if (evt.button == 0)
			{
				_pickerControlID = GUIUtility.GetControlID(FocusType.Passive);

				EditorGUIUtility.ShowObjectPicker<Object>(
					_value,
					false,
					"",
					_pickerControlID
				);

				EditorApplication.update += WaitForPicker;
			}
		}

		private void WaitForPicker()
		{
			if (Event.current == null) 
				return;

			if (Event.current.commandName == "ObjectSelectorUpdated")
			{
				if (EditorGUIUtility.GetObjectPickerControlID() == _pickerControlID)
				{
					value = EditorGUIUtility.GetObjectPickerObject();
				}
			}

			if (Event.current.commandName == "ObjectSelectorClosed")
			{
				EditorApplication.update -= WaitForPicker;
			}
		}
	}

	public class ObjectDisplay : VisualElement
	{
		private static readonly string ussClassName = "unity-object-field-display";
		private static readonly string iconUssClassName = ussClassName + "__icon";
		internal static readonly string labelUssClassName = ussClassName + "__label";
		private static readonly string acceptDropVariantUssClassName = ussClassName + "--accept-drop";
		

		private BaseField<Object> m_ObjectField;
		private readonly Image m_ObjectIcon;
		private readonly Label m_ObjectLabel;

		public bool IsObjectMissing { get; private set; }

		public ObjectDisplay(BaseField<Object> objectField)
		{
			AddToClassList(ussClassName);
			m_ObjectIcon = new Image
			{
				scaleMode = ScaleMode.ScaleAndCrop,
				pickingMode = PickingMode.Ignore
			};
			m_ObjectIcon.AddToClassList(iconUssClassName);
			m_ObjectLabel = new Label
			{
				pickingMode = PickingMode.Ignore
			};
			m_ObjectLabel.AddToClassList(labelUssClassName);
			m_ObjectField = objectField;
			Update();
			Add(m_ObjectIcon);
			Add(m_ObjectLabel);
		}

		private void Update()
		{
			IsObjectMissing = false;
			SerializedProperty serializedProperty = null; // m_ObjectField.GetProperty(serializedPropertyKey) as SerializedProperty;
			if (serializedProperty != null)
			{
				if (IsPropertyValid(serializedProperty) == false)
				{
					//m_ObjectField.SetProperty(serializedPropertyKey, null);
					return;
				}

				//IsObjectMissing = IsMissingObjectReference(serializedProperty);
			}

			//GUIContent gUIContent = EditorGUIUtility.ObjectContent(m_ObjectField.value, m_ObjectField.objectType, serializedProperty);
			//m_ObjectIcon.image = gUIContent.image;
			//m_ObjectLabel.text = gUIContent.text;
		}

		private bool IsPropertyValid(SerializedProperty property)
		{
			return property != null && property.objectReferenceInstanceIDValue != 0;
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

				var toggle = new Toggle("Show Object Selector Button");

				var interfaceField = new InterfaceField(property.displayName);
				//container.Add(interfaceField);


				// Znajdź przycisk selektora
				var selectorButton = objectField.Q(className: ObjectField.selectorUssClassName);

				if (selectorButton != null)
				{
					selectorButton.style.display = DisplayStyle.None;
				}

				var visualInput = objectField.Q(className: ObjectField.inputUssClassName);
				visualInput.Add(objectSelectorButton);
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
