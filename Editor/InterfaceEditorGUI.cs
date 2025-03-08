using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bipolar.Editor
{
	public static class InterfaceEditorGUI
	{
		public const float InterfaceSelectorButtonWidth = 20;

		private static int s_ObjectFieldHash = "s_ObjectFieldHash".GetHashCode();

		private static GUIStyle buttonStyle = EditorStyles.objectField;

		public static Object InterfaceField(Rect position, GUIContent label, Object @object, System.Type interfaceType, bool allowSceneObjects)
		{
			var currentEvent = Event.current;
			var eventType = currentEvent.type;

			int id = GUIUtility.GetControlID(hint: s_ObjectFieldHash, FocusType.Keyboard, position);
			position = EditorGUI.PrefixLabel(position, id, label);

			position = GetObjectFieldThumbnailRect(position, interfaceType);

			var iconSize = EditorGUIUtility.GetIconSize();

			//switch (eventType)
			//{
			//	case EventType.Repaint:

			//		EditorGUIUtility.ObjectContent(@object, interfaceType);
			//		Rect position2 = buttonStyle.margin.Remove(GetButtonRect(objectFieldVisualType, position));
			//		buttonStyle.Draw(position2, GUIContent.none, id, DragAndDrop.activeControlID == id, position2.Contains(Event.current.mousePosition));
			//		break;
			//}
			EditorGUIUtility.SetIconSize(iconSize);
			return @object;
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









		public static void InterfaceField(Rect rect, GUIContent label, SerializedProperty serializedObjectProperty, System.Type interfaceType, bool allowSceneObjects)
		{
			//var objectFieldRect = new Rect(rect.x, rect.y, rect.width - InterfaceSelectorButtonWidth, rect.height);
			//var interfaceButtonRect = new Rect(rect.x + objectFieldRect.width, rect.y, InterfaceSelectorButtonWidth, rect.height);
			//serializedObjectProperty.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, label, serializedObjectProperty.objectReferenceValue, interfaceType, allowSceneObjects);

			if (interfaceType == default)
				return;

			var currentEvent = Event.current;
			var eventType = currentEvent.type;
			int id = GUIUtility.GetControlID(hint: s_ObjectFieldHash, FocusType.Keyboard, rect);

			var @object = serializedObjectProperty.objectReferenceValue;


			switch (eventType)
			{
				case EventType.Repaint:
						var tempContent = EditorGUIUtility.ObjectContent(serializedObjectProperty.objectReferenceValue, interfaceType);
						buttonStyle.Draw(EditorGUI.IndentedRect(rect), tempContent, 1, DragAndDrop.activeControlID == id, rect.Contains(Event.current.mousePosition));

						//	if (GUI.Button(interfaceButtonRect, "I"))
						//	{
						//		Object objectReferenceValue = serializedObjectProperty.objectReferenceValue;
						//		InterfaceSelectorWindow.Show(interfaceType, objectReferenceValue, (obj) => AssignValue(serializedObjectProperty, obj));
						//	}
						break;

				case EventType.DragExited:
					if (GUI.enabled)
						HandleUtility.Repaint();
					
					break;
			}



			AssignValue(serializedObjectProperty, @object);


			static void AssignValue(SerializedProperty property, Object @object)
			{
				property.objectReferenceValue = @object;
				property.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
