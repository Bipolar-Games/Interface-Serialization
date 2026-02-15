using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bipolar.Editor
{
	public static class InterfaceEditorUtility
	{
		public const string AddComponentButtonText = "Add";
		public const string CreateAssetButtonText = "Create";

		public static System.Type GetRequiredType(FieldInfo fieldInfo)
		{
			var type = fieldInfo.FieldType;
			while (type != null)
			{
				if (type.IsArray)
					type = type.GetElementType();

				if (type == null)
					return null;

				if (type.IsGenericType)
				{
					if (type.GetGenericTypeDefinition() == typeof(Serialized<,>))
						return type.GetGenericArguments()[0];

					if (typeof(IEnumerable).IsAssignableFrom(type))
					{
						type = type.GetGenericArguments()?[0];
						continue;
					}
				}

				type = type.BaseType;
			}

			return null;
		}

        public static ObjectCreationTypes GetButtons(FieldInfo fieldInfo, ObjectCreationTypes @default = ObjectCreationTypes.None)
        {
            var buttonsAttribute = fieldInfo.GetCustomAttribute<NewObjectButtonAttribute>();

            var buttons = buttonsAttribute?.ButtonType ?? @default;
            return buttons;
        }

        public static string GetAssetFileName(System.Type type)
        {
            var attribute = type.GetCustomAttribute<CreateAssetMenuAttribute>();
            if (attribute != null)
            {
                string fileName = attribute.fileName;
                if (string.IsNullOrWhiteSpace(fileName) == false)
                    return fileName;
            }

            return "New " + ObjectNames.NicifyVariableName(type.Name);
        }

		internal static AddComponentDropdown ShowAddComponentDropDown(System.Type type, SerializedProperty objectProperty, Rect dropdownRect)
		{
			var dropdown = new AddComponentDropdown(type);
			dropdown.OnItemSelected += item =>
			{
				var gameObjects = objectProperty.serializedObject.targetObjects
					.Where(obj => obj is Component)
					.Select(c => ((Component)c).gameObject);

				foreach (var gameObject in gameObjects)
				{
					var addedComponent = ObjectFactory.AddComponent(gameObject, item.Type);
					objectProperty.objectReferenceValue = addedComponent;
					objectProperty.serializedObject.ApplyModifiedProperties();
				}
			};

			dropdown.Show(dropdownRect);
			return dropdown;
		}

		internal static CreateAssetDropdown ShowCreateAssetDropdown(System.Type type, SerializedProperty objectProperty, Rect dropdownRect)
		{
			var dropdown = new CreateAssetDropdown(type);
			dropdown.OnItemSelected += item =>
			{
				var createdAsset = ObjectFactory.CreateInstance(item.Type);

				string folderPath = "Assets";
				var selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);
				if (selectedAssets.Length > 0)
				{
					folderPath = AssetDatabase.GetAssetPath(selectedAssets[0]);
					if (AssetDatabase.IsValidFolder(folderPath) == false)
						folderPath = Path.GetDirectoryName(folderPath);
				}

				var preferedAssetName = GetAssetFileName(item.Type);
				var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{preferedAssetName}.asset");
				AssetDatabase.CreateAsset(createdAsset, assetPath);
				AssetDatabase.SaveAssets();
				EditorGUIUtility.PingObject(createdAsset);
				objectProperty.objectReferenceValue = createdAsset;
				objectProperty.serializedObject.ApplyModifiedProperties();
			};

			dropdown.Show(dropdownRect);
			return dropdown;
		}

        public static Rect GetDropdownRect(Rect fieldRect)
        {
            var dropdownRect = new Rect();
            dropdownRect.width = Mathf.Max(Screen.width / 2f, 230);
            dropdownRect.height = fieldRect.height;
            dropdownRect.center = fieldRect.center;
            return dropdownRect;
        }
	}
}
