﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Bipolar.Editor
{
    public static class InterfaceTypesCache
    {

    }

    public class InterfaceSelectorWindow : EditorWindow
    {
        private class InterfacePickerWindowData
        {
            public System.Type filteredType;
            public bool isFocused = false;
            public static int tab;

            private Object[] assetsOfType;
			public Object[] AssetsOfType => assetsOfType;

			public InterfacePickerWindowData(System.Type interfaceType)
            {
				filteredType = interfaceType;
                assetsOfType = GetAssetsOfType(interfaceType).ToArray();
            }
        }

        #region Constants
        private const string noneObjectName = "None";
        private const string searchBoxName = "searchBox";
        private static readonly string[] tabs = { "Assets", "Scene" };
        private static readonly GUILayoutOption[] tabsLayout = { GUILayout.MaxWidth(110) };
        private static GUIStyle _selectedStyle;
        private static GUIStyle SelectedStyle
        {
            get
            {
                if (_selectedStyle == null || _selectedStyle.normal.background == null)
                {
                    var backgroundTexture = new Texture2D(1, 1);
                    backgroundTexture.SetPixel(0, 0, new Color32(62, 95, 150, 255));
                    backgroundTexture.Apply();

                    _selectedStyle = new GUIStyle(EditorStyles.label);
                    _selectedStyle.name = "Selected";
                    _selectedStyle.normal.textColor = Color.white;
                    _selectedStyle.normal.background = backgroundTexture;
                }
                return _selectedStyle;
            }
        }

        private const int AssetsTab = 0;
        private const int SceneObjectsTab = 1;

        #endregion

        private readonly static Dictionary<System.Type, InterfacePickerWindowData> windowsByType = new Dictionary<System.Type, InterfacePickerWindowData>();

        private InterfacePickerWindowData data;
        private float assetsViewScrollAmount;
        private float sceneObjectsViewScrollAmount;
        private Object selectedObject;
        private string searchFilter = "";
        private Component[] componentsOfInterface;
        private System.Action<Object> OnClosed;

        public static void Show(System.Type interfaceType, Object selectedObject, System.Action<Object> onClosed = null)
        {
            var window = Get(interfaceType);
            window.selectedObject = selectedObject;
            window.ShowUtility();
            window.OnClosed = onClosed;
        }

        private static InterfaceSelectorWindow Get(System.Type interfaceType)
        {
            var window = CreateInstance<InterfaceSelectorWindow>();
            window.titleContent = new GUIContent($"Select {interfaceType.Name}");
            window.data = GetData(interfaceType);
            window.componentsOfInterface = GetComponentsOfInterface(interfaceType);
            return window;
        }

        private static InterfacePickerWindowData GetData(System.Type interfaceType)
        {
            var newData = new InterfacePickerWindowData(interfaceType);
            return newData;
        }

        private static Component[] GetComponentsOfInterface(System.Type interfaceType)
		{
			GameObject[] allGameObjects = GetAllGameObject();

			var componentsOfInterface = new List<Component>();

			var tempComponents = new List<Component>();
			foreach (var gameObject in allGameObjects)
			{
				tempComponents.Clear();
				gameObject.GetComponents(tempComponents);
                foreach (var component in tempComponents)
                    if (interfaceType.IsAssignableFrom(component.GetType()))
                        componentsOfInterface.Add(component);
            }

			return componentsOfInterface.ToArray();
		}

		private static GameObject[] GetAllGameObject()
		{
			GameObject[] allGameObjects;
			var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage)
			{
				var prefabRoot = prefabStage.prefabContentsRoot;
				allGameObjects = prefabRoot.GetComponentsInChildren<Transform>()
					.Select(tf => tf.gameObject)
					.ToArray();
			}
			else
			{
				allGameObjects = FindObjectsOfType<GameObject>(true);
			}

			return allGameObjects;
		}

		private void OnGUI()
        {
            GUI.SetNextControlName(searchBoxName);
            searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);
            if (data.isFocused == false)
            {
                GUI.FocusControl(searchBoxName);
                data.isFocused = true;
            }

            int tab = InterfacePickerWindowData.tab;
			tab = GUILayout.Toolbar(tab, tabs, tabsLayout);

            switch (tab)
            {
                case AssetsTab:
                    DrawAssetsPanel();
                    break;

                case SceneObjectsTab:
                    DrawSceneObjectsPanel();
                    break;
            }
            InterfacePickerWindowData.tab = tab;
        }

        private void DrawAssetsPanel()
        {
            assetsViewScrollAmount = EditorGUILayout.BeginScrollView(new Vector2(0, assetsViewScrollAmount)).y;

            EditorGUIUtility.SetIconSize(new Vector2(16, 16));
            var pressedObject = selectedObject;
            if (DrawAssetListItem(null))
            {
                pressedObject = null;
            }
            foreach (var asset in data.AssetsOfType)
            {
                if (asset.name.ToLower().Contains(searchFilter.ToLower()))
                {
                    if (asset is ScriptableObject scriptableObject)
                    {
                        if (DrawAssetListItem(scriptableObject))
                        {
                            pressedObject = asset;
                        }
                    }
                    else if (asset is Component component)
                    {
                        if (DrawComponentListItem(component))
                        {
                            pressedObject = asset;
                        }
                    }
                }
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
            EditorGUILayout.EndScrollView();

            if (selectedObject == pressedObject && Event.current.clickCount > 1)
            {
                Event.current.clickCount = 0;
                Close();
            }

            selectedObject = pressedObject;
        }

        private void DrawSceneObjectsPanel()
        {
            sceneObjectsViewScrollAmount = EditorGUILayout.BeginScrollView(new Vector2(0, sceneObjectsViewScrollAmount)).y;
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));
            var pressedObject = selectedObject;

            if (DrawComponentListItem(null))
                pressedObject = null;
            
            foreach (var component in componentsOfInterface)
            {
                if (component.name.ToLower().Contains(searchFilter.ToLower()))
                {
                    if (DrawComponentListItem(component))
                    {
                        pressedObject = component;
                    }
                }
            }

            EditorGUIUtility.SetIconSize(Vector2.zero);
            EditorGUILayout.EndScrollView();

            if (selectedObject == pressedObject && Event.current.clickCount > 1)
            {
                Event.current.clickCount = 0;
                Close();
            }

            selectedObject = pressedObject;
        }

        private bool DrawAssetListItem(ScriptableObject asset)
        {
            bool wasPressed = false;
            GUILayout.BeginHorizontal();

            var image = AssetPreview.GetMiniThumbnail(asset);
            GUILayout.Space(image ? 20 : 36);
            string name = asset ? asset.name : noneObjectName;
            var style = asset == selectedObject ? SelectedStyle : EditorStyles.label;
            if (GUILayout.Button(new GUIContent(name, image), style))
                wasPressed = true;

            GUILayout.EndHorizontal();
            return wasPressed;
        }

        private bool DrawComponentListItem(Component component)
        {
            bool wasPressed = false;
            GUILayout.BeginHorizontal();
            var objectContent = EditorGUIUtility.ObjectContent(component, typeof(GameObject));
            objectContent.text = component 
                ? $"{component.name} ({ObjectNames.NicifyVariableName(component.GetType().Name)})"
                : noneObjectName;
            var style = EditorStyles.label;
            
            if (selectedObject == component)
                style = SelectedStyle;

            GUILayout.Space(20);
            if (GUILayout.Button(objectContent, style))
                wasPressed = true;

            GUILayout.EndHorizontal();
            return wasPressed;
        }

        public static ICollection<Object> GetAssetsOfType(System.Type type)
        {
            //var derivedTypes = TypeCache.GetTypesDerivedFrom(type);
            //foreach (var derivedType in derivedTypes)
            //{
            //    if (derivedType.IsSubclassOf(typeof(ScriptableObject)))
            //        filterBuilder.Append($"t:{derivedType.Name} ");
            //}
            //if (filterBuilder.Length < 1)

            var foundObjectsList = new List<Object>();

            var allScriptableObjectsGuids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");
            foreach (var assetGuid in allScriptableObjectsGuids)
            {
                var assetFilePath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetFilePath);
                if (asset == null)
                    continue;
                else if (type.IsAssignableFrom(asset.GetType()) == false)
                    continue;

                foundObjectsList.Add(asset);
            }

			var allPrefabsGuids = AssetDatabase.FindAssets("t:Prefab");
			foreach (var assetGuid in allPrefabsGuids)
			{
				var prefabFilePath = AssetDatabase.GUIDToAssetPath(assetGuid);
				var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFilePath);
				if (prefab == null)
					continue;

                var components = prefab.GetComponents(type);
                foundObjectsList.AddRange(components);
			}
			return foundObjectsList;
        }

        private void OnLostFocus()
        {
            Close();
        }

        private void OnDestroy()
        {
            OnClosed?.Invoke(selectedObject);
            OnClosed = null;
        }
    }
}
