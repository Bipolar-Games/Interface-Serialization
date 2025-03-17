using UnityEditor;
using UnityEngine;

using Styles = Bipolar.Editor.InterfaceEditorStyles;

namespace Bipolar.Editor
{
	public static class InterfaceEditorGUI
	{
		public const float InterfaceSelectorButtonWidth = 19;

		private static readonly int objectFieldHash = "s_ObjectFieldHash".GetHashCode();

		public static Object InterfaceField(Rect position, GUIContent label, Object @object, System.Type interfaceType, bool allowSceneObjects = true) => DoInterfaceField(position, label, null, @object, interfaceType, allowSceneObjects);

		public static void InterfaceField(Rect position, GUIContent label, SerializedProperty serializedObjectProperty, System.Type interfaceType, bool allowSceneObjects = true) => DoInterfaceField(position, label, serializedObjectProperty, null, interfaceType, allowSceneObjects);

		private static Object DoInterfaceField(Rect position, GUIContent label, SerializedProperty serializedObjectProperty, Object @object, System.Type interfaceType, bool allowSceneObjects)
		{
			if (interfaceType == default)
				throw new System.ArgumentNullException("Missing type of interface");

			if (serializedObjectProperty == default && @object == default)
				throw new System.ArgumentNullException("Missing serialized object");

			int controlID = GUIUtility.GetControlID(hint: objectFieldHash, FocusType.Keyboard, position);

			position = EditorGUI.PrefixLabel(position, controlID, label);

			using var scope = new IconSizeScope(12, 12);

			var currentEvent = Event.current;
			var eventType = currentEvent.type;

			if (serializedObjectProperty != null)
				@object = serializedObjectProperty.objectReferenceValue;

			var validatedObject = GetValidatedObject(@object, interfaceType);
			if (@object != validatedObject)
				AssignValue(validatedObject);

			switch (eventType)
			{
				case EventType.Repaint:
					Repaint();
					break;

				case EventType.MouseDown:
					HandleMousePress();
					break;

				case EventType.DragUpdated:
				case EventType.DragPerform:
					HandleDragging();
					break;

				case EventType.DragExited:
					if (GUI.enabled)
						HandleUtility.Repaint();
					break;

				case EventType.ValidateCommand:
					ValidateEventCommand();
					break;

				case EventType.ExecuteCommand:
					ExecuteEventCommand();
					break;

				case EventType.KeyDown:
					if (GUIUtility.keyboardControl == controlID)
						HandleKeyboardPress();
					break;
			}

			return @object;

			void Repaint()
			{
				var tempContent = EditorGUIUtility.ObjectContent(@object, interfaceType);
				var mousePosition = currentEvent.mousePosition;
				bool isDragged = DragAndDrop.activeControlID == controlID;
				Styles.ObjectField.Draw(EditorGUI.IndentedRect(position), tempContent, controlID, isDragged, position.Contains(mousePosition));

				var buttonStyle = Styles.ObjectSelectorButton;
				var selectorButtonRect = buttonStyle.margin.Remove(GetButtonRect(position));
				buttonStyle.Draw(selectorButtonRect, GUIContent.none, controlID, isDragged, selectorButtonRect.Contains(mousePosition));
			}

			void HandleMousePress()
			{
				var mousePosition = currentEvent.mousePosition;
				if (currentEvent.button != 0 || position.Contains(mousePosition) == false)
					return;

				var selectorButtonRect = GetButtonRect(position);
				EditorGUIUtility.editingTextField = false;
				if (selectorButtonRect.Contains(mousePosition))
				{
					HandleSelectorButtonPress();
				}
				else
				{
					HandleObjectFieldPress();
				}

				void HandleSelectorButtonPress()
				{
					if (GUI.enabled)
					{
						GUIUtility.keyboardControl = controlID;
						InterfaceSelectorWindow.Show(interfaceType, @object, AssignValue);
					}
				}

				void HandleObjectFieldPress()
				{
					var clickedObject = @object;
					if (clickedObject is Component component)
						clickedObject = component.gameObject;

					if (EditorGUI.showMixedValue)
						return;

					switch (currentEvent.clickCount)
					{
						case 1:
							GUIUtility.keyboardControl = controlID;
							EditorGUIUtility.PingObject(clickedObject);
							currentEvent.Use();
							break;

						case 2:
							AssetDatabase.OpenAsset(clickedObject);
							currentEvent.Use();
							GUIUtility.ExitGUI();
							break;
					}
				}
			}

			void HandleDragging()
			{
				if (GUI.enabled == false)
					return;

				if (position.Contains(currentEvent.mousePosition) == false)
					return;

				var objectReferences = DragAndDrop.objectReferences;
				if (objectReferences.Length < 1)
					return;

				var draggedObject = GetValidatedObject(objectReferences[0], interfaceType);
				if (draggedObject == null)
					return;

				if (allowSceneObjects == false && EditorUtility.IsPersistent(draggedObject) == false)
					return;

				if (DragAndDrop.visualMode == DragAndDropVisualMode.None)
					DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

				bool performed = currentEvent.type == EventType.DragPerform;
				if (performed)
				{
					@object = draggedObject;
					AssignValue(draggedObject);
					GUI.changed = true;
					DragAndDrop.AcceptDrag();
				}

				DragAndDrop.activeControlID = performed ? 0 : controlID;
				currentEvent.Use();
			}

			void ValidateEventCommand()
			{
				if (GUIUtility.keyboardControl == controlID && IsDeleteCommand(currentEvent.commandName))
					currentEvent.Use();
			}

			void ExecuteEventCommand()
			{
				if (GUIUtility.keyboardControl != controlID)
					return;

				if (IsDeleteCommand(currentEvent.commandName))
				{
					@object = null;
					AssignValue(null);
					GUI.changed = true;
					currentEvent.Use();
				}
			}

			static Object GetValidatedObject(Object obj, System.Type type)
			{
				if (obj == null)
					return null;

				if (type.IsAssignableFrom(obj.GetType()))
					return obj;

				if (obj is GameObject gameObject)
					return gameObject.GetComponent(type);

				return null;
			}

			void HandleKeyboardPress()
			{
				if (IsConfirmKeyboardEvent(currentEvent))
				{
					InterfaceSelectorWindow.Show(interfaceType, @object, AssignValue);
					currentEvent.Use();
					GUIUtility.ExitGUI();
				}
			}

			void AssignValue(Object assignedObject)
			{
				@object = assignedObject;
				serializedObjectProperty.objectReferenceValue = assignedObject;
				serializedObjectProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		private static bool IsDeleteCommand(string commandName) =>
			commandName == "Delete" || commandName == "SoftDelete";

		private static Rect GetButtonRect(Rect position) => new Rect(
			position.xMax - InterfaceSelectorButtonWidth,
			position.y,
			InterfaceSelectorButtonWidth,
			position.height);

		private static bool IsConfirmKeyboardEvent(Event @event)
		{
			if (@event.alt || @event.command || @event.control || @event.shift)
				return false;

			var key = @event.keyCode;
			switch (key)
			{
				case KeyCode.Return:
				case KeyCode.Space:
				case KeyCode.KeypadEnter:
					return true;
				default:
					return false;
			}
		}

		internal struct IconSizeScope : System.IDisposable
		{
			private readonly Vector2 originalIconSize;

			public IconSizeScope(Vector2 iconSize)
			{
				originalIconSize = EditorGUIUtility.GetIconSize();
				EditorGUIUtility.SetIconSize(iconSize);
			}

			public IconSizeScope(float x, float y)
				: this(new Vector2(x, y))
			{ }

			public readonly void Dispose() => EditorGUIUtility.SetIconSize(originalIconSize);
		}
	}
}
