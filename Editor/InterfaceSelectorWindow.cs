using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Bipolar.Editor
{
    public class InterfaceSelectorWindow : EditorWindow
    {
        private class InterfacePickerWindowData
        {
            public System.Type filteredType;
            public bool isFocused = false;
            public int tab;
            public ScriptableObject[] assetsOfType;

            public InterfacePickerWindowData(System.Type interfaceType)
            {
                filteredType = interfaceType;
                assetsOfType = GetAssetsOfType(interfaceType);
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
            if (windowsByType.TryGetValue(interfaceType, out var existingData))
            {
                if (existingData != null)
                {
                    existingData.isFocused = false;
                    return existingData;
                }
            }

            var newData = new InterfacePickerWindowData(interfaceType);
            windowsByType[interfaceType] = newData;
            return newData;
        }

        private static Component[] GetComponentsOfInterface(System.Type interfaceType)
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

            var componentsOfInterface = new List<Component>();

            var tempComponents = new List<Component>();
            foreach (var gameObject in allGameObjects)
            {
                tempComponents.Clear();
                gameObject.GetComponents(interfaceType, tempComponents);
                foreach (var component in tempComponents)
                    componentsOfInterface.Add(component);
            }

            return componentsOfInterface.ToArray();
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

            data.tab = GUILayout.Toolbar(data.tab, tabs, tabsLayout);

            switch (data.tab)
            {
                case AssetsTab:
                    DrawAssetsPanel();
                    break;

                case SceneObjectsTab:
                    DrawSceneObjectsPanel();
                    break;
            }
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
            foreach (var asset in data.assetsOfType)
            {
                if (asset.name.ToLower().Contains(searchFilter.ToLower()))
                {
                    if (DrawAssetListItem(asset))
                    {
                        pressedObject = asset;
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
			//var listOfItems = new List<(GUIContent label, Object @object)>();
			//listOfItems.Add((new GUIContent(noneObjectName, null, null), null));

            if (DrawSceneObjectListItem(null))
                pressedObject = null;
            
            foreach (var component in componentsOfInterface)
            {
                if (component.name.ToLower().Contains(searchFilter.ToLower()))
                {
                    if (DrawSceneObjectListItem(component))
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

        private bool DrawSceneObjectListItem(Component component)
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


        /// <summary>
        /// Used to get assets of a certain type and file extension from entire project
        /// </summary>
        /// <param name="type">The type to retrieve. eg typeof(GameObject).</param>
        /// <param name="fileExtension">The file extention the type uses eg ".prefab".</param>
        /// <returns>An Object array of assets.</returns>
        public static ScriptableObject[] GetAssetsOfType(System.Type type, string fileExtension = "asset")
        {
            var derivedTypes = TypeCache.GetTypesDerivedFrom(type);
            var filterBuilder = new StringBuilder();
            foreach (var derivedType in derivedTypes)
            {
                if (derivedType.IsSubclassOf(typeof(ScriptableObject)))
                    filterBuilder.Append($"t:{derivedType.Name} ");
            }
            if (filterBuilder.Length < 1)
                filterBuilder.Append("t:");

            var foundObjectsList = new List<ScriptableObject>();

            var assetsGuids = AssetDatabase.FindAssets(filterBuilder.ToString());
            foreach (var assetGuid in assetsGuids)
            {
                var assetFilePath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetFilePath);
                if (asset == null)
                    continue;
                else if (type.IsAssignableFrom(asset.GetType()) == false)
                    continue;

                foundObjectsList.Add(asset);
            }
            return foundObjectsList.ToArray();
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
