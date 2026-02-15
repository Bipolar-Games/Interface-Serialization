using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bipolar.Editor
{
	public static class InterfaceEditorStyles
	{
		public static readonly GUIStyle ObjectField = EditorStyles.objectField;
		public static readonly GUIStyle ObjectSelectorButton =
				(GUIStyle)typeof(EditorStyles).GetProperty("objectFieldButton", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
	}
}
