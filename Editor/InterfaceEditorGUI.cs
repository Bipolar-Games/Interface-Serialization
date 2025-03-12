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
				return default;

			if (serializedObjectProperty == default && @object == default)
				return default;

			using var scope = new IconSizeScope(12, 12);

			var currentEvent = Event.current;
			var eventType = currentEvent.type;
			int id = GUIUtility.GetControlID(hint: objectFieldHash, FocusType.Keyboard, position);

			if (serializedObjectProperty != null)
				@object = serializedObjectProperty.objectReferenceValue;

			switch (eventType)
			{
				case EventType.MouseDown:
					HandleMouseDown();
					break;

				case EventType.Repaint:
					var tempContent = EditorGUIUtility.ObjectContent(serializedObjectProperty.objectReferenceValue, interfaceType);
					var mousePosition = currentEvent.mousePosition;
					Styles.ObjectField.Draw(EditorGUI.IndentedRect(position), tempContent, 1, DragAndDrop.activeControlID == id, position.Contains(mousePosition));

					var buttonStyle = Styles.ObjectSelectorButton;
					var selectorButtonRect = buttonStyle.margin.Remove(GetButtonRect(position));
					buttonStyle.Draw(selectorButtonRect, GUIContent.none, id, DragAndDrop.activeControlID == id, selectorButtonRect.Contains(mousePosition));
					break;

				case EventType.DragExited:
					if (GUI.enabled)
						HandleUtility.Repaint();

					break;
			}





			AssignValue(serializedObjectProperty, @object);
			return @object;

			static void AssignValue(SerializedProperty property, Object @object)
			{
				property.objectReferenceValue = @object;
				property.serializedObject.ApplyModifiedProperties();
			}

			void HandleMouseDown()
			{
				var mousePosition = currentEvent.mousePosition;
				if (currentEvent.button == 0 && position.Contains(mousePosition))
				{
					var selectorButtonRect = GetButtonRect(position);
					EditorGUIUtility.editingTextField = false;
					if (selectorButtonRect.Contains(mousePosition))
					{
						if (GUI.enabled)
						{

						}


					}


				}
			}
		}


		private static Rect GetObjectFieldThumbnailRect(Rect position, System.Type objType)
		{
			if (EditorGUIUtility.HasObjectThumbnail(objType) && position.height > 18f)
			{
				float size = Mathf.Min(position.width, position.height);
				position.height = size;
				position.xMin = position.xMax - size;
			}

			return position;
		}


		private static Rect GetButtonRect(Rect position) => new Rect(
			position.xMax - InterfaceSelectorButtonWidth,
			position.y,
			InterfaceSelectorButtonWidth,
			position.height);

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
