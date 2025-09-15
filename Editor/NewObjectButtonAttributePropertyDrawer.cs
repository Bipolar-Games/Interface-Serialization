using UnityEditor;
using UnityEngine;

namespace Bipolar.Editor
{
	[CustomPropertyDrawer(typeof(NewObjectButtonAttribute), true)]
	public class NewObjectButtonAttributePropertyDrawer : PropertyDrawer
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			//if (property.propertyType != SerializedPropertyType.ObjectReference)
			{
				EditorGUI.PropertyField(position, property, label, true);
				return;
			}


			var buttonAttribute = (NewObjectButtonAttribute)attribute;




		}
	}
}
