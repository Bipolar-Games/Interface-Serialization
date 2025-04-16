using UnityEditor;
using UnityEngine;

namespace Bipolar.Editor
{
	[CustomPropertyDrawer(typeof(Serialized<,>), useForChildren: true)]
	public class SerializedInterfaceDrawer : PropertyDrawer
	{
		private const string serializedObjectPropertyName = "serializedObject";
		private static readonly GUIContent EmptyLabel = new GUIContent(" ");

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			string propertyPath = property.propertyPath;
			var next = property.Copy();
			next.NextVisible(true);
			next.NextVisible(true);
			var parentPath = GetPropertyParentPath(next);

			if (parentPath == propertyPath)
			{
				var foldoutRect = new Rect(position.x, position.y, 20, position.height);
				property.isExpanded = EditorGUI.Foldout(
					foldoutRect,
					property.isExpanded,
					label,
					toggleOnLabelClick: true);

				DrawProperty(position, property, EmptyLabel);
				if (property.isExpanded)
				{
					EditorGUI.indentLevel++;
					var iterator = property.Copy();
					iterator.NextVisible(true);
					while (iterator.NextVisible(true) && iterator.propertyPath.Contains(propertyPath))
					{
						var subpropertyPath = iterator.propertyPath;
						if (subpropertyPath.Contains(propertyPath) == false)
							break;

						if (GetPropertyParentPath(subpropertyPath) == propertyPath)
						{
							var iteratorLabel = new GUIContent(iterator.displayName);
							float h = EditorGUI.GetPropertyHeight(iterator, iteratorLabel, true);
							var rect = EditorGUILayout.GetControlRect(hasLabel: false, h);
							EditorGUI.PropertyField(rect, iterator, iteratorLabel);
						}
					}
					EditorGUI.indentLevel--;
				}
			}
			else
			{
				DrawProperty(position, property, label);
			}

			EditorGUI.EndProperty();
		}

		private void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
		{
			var serializedObjectProperty = property.FindPropertyRelative(serializedObjectPropertyName);
			var requiredType = InterfaceEditorUtility.GetRequiredType(fieldInfo);
			InterfaceEditorGUI.InterfaceField(position, label, serializedObjectProperty, requiredType);
		}

		private static string GetPropertyParentPath(SerializedProperty property) => GetPropertyParentPath(property.propertyPath);

		private static string GetPropertyParentPath(string propertyPath)
		{
			int lastDotIndex = propertyPath.LastIndexOf('.');
			if (lastDotIndex < 0)
				return null;

			return propertyPath.Substring(0, lastDotIndex);
		}
	}
}
