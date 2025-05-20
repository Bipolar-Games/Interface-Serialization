using System;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Editor
{
    internal class TypeItem : AdvancedDropdownItem
    {
        public Type Type { get; }

        public TypeItem(Type type) : this(type, ObjectNames.NicifyVariableName(type.Name))
        { }

        public TypeItem(Type type, string name) : base(name)
        {
            Type = type;
        }
    }

    internal class AddComponentDropdown : AdvancedDropdown
    {
        private static readonly Dictionary<Type, AddComponentDropdown> cachedPopups = new Dictionary<Type, AddComponentDropdown>();

        public event Action<TypeItem> OnItemSelected;

        public Type SubcomponentType { get; private set; }

        private readonly AdvancedDropdownItem root;

        public static AddComponentDropdown Get(Type subcomponentType)
        {
            if (cachedPopups.TryGetValue(subcomponentType, out var popup) == false)
            {
                popup = new AddComponentDropdown(subcomponentType);
                cachedPopups.Add(subcomponentType, popup);
            }
            popup.OnItemSelected = null;
            return popup;
        }

        private AddComponentDropdown(Type requiredType) : base(new AdvancedDropdownState())
        {
            SubcomponentType = requiredType;
            var types = TypeCache.GetTypesDerivedFrom(SubcomponentType)
                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(Component)));
            var builder = new TypeItemBuilder(GetRootItemName());
            PopulateBuilderWithTypes(builder, types);
            root = builder.Build();
        }

        protected virtual string GetRootItemName() => "Component";

        private void PopulateBuilderWithTypes(TypeItemBuilder builder, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AddTypeToBuilder(builder, type);
            }
        }

        protected void AddTypeToBuilder(TypeItemBuilder builder, Type type)
        {
            if (type.IsDefined(typeof(AddComponentMenu), true))
            {
                var attribute = (AddComponentMenu)type.GetCustomAttributes(typeof(AddComponentMenu), true)[0];
                var path = attribute.componentMenu;
                string[] pathItems = path.Contains('/')
                    ? path.Split('/')
                    : path.Split('\\');

                var componentName = pathItems[pathItems.Length - 1];
                if (string.IsNullOrWhiteSpace(componentName) == false)
                {
                    builder.AddType(type, pathItems, attribute.componentOrder);
                    return;
                }
            }
            builder.AddType(type);
        }

        protected override AdvancedDropdownItem BuildRoot() => root;

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is TypeItem typeItem)
            {
                OnItemSelected?.Invoke(typeItem);
            }
        }
    }

    public class TypeItemBuilder
    {
        internal class Node
        {
            public string Name { get; set; }

            private readonly List<Node> subfolders = new List<Node>();
            private readonly List<(TypeItem item, int order)> items = new List<(TypeItem item, int order)>();

            public Node()
            { }

            public Node(string name) : this()
            {
                Name = name;
            }

            public void AddItem(Type type, string name, int order)
            {
                items.Add((new TypeItem(type, name), order));
                items.Sort((lhs, rhs) => lhs.order.CompareTo(rhs.order));
            }

            public void AddItem(Type type) => items.Add((new TypeItem(type), 20));

            public Node GetOrAddSubfolder(string name)
            {
                int index = subfolders.FindIndex(sub => sub.Name == name);
                if (index >= 0)
                    return subfolders[index];

                var newSubfolder = new Node(name);
                subfolders.Add(newSubfolder);
                subfolders.Sort((lhs, rhs) => lhs.Name.CompareTo(rhs.Name));
                return newSubfolder;
            }

            public void Clear()
            {
                Name = null;
                subfolders.Clear();
                items.Clear();
            }

            public AdvancedDropdownItem Build()
            {
                var builtItem = new AdvancedDropdownItem(Name);
                foreach (var subfolder in subfolders)
                    builtItem.AddChild(subfolder.Build());

                foreach (var (item, _) in items)
                    builtItem.AddChild(item);

                return builtItem;
            }
        }

        private readonly Node root = new Node();

        public string Name
        {
            get => root.Name;
            set => root.Name = value;
        }

        public TypeItemBuilder(string name = null)
        {
            Name = name;
        }

        public void AddType(Type type, string[] path, int order)
        {
            var node = root;
            for (int i = 0; i < path.Length - 1; i++)
                node = node.GetOrAddSubfolder(path[i]);

            node.AddItem(type, path[path.Length - 1], order);
        }

        public void AddType(Type type) => root.AddItem(type);

        public void Clear() => root.Clear();

        public AdvancedDropdownItem Build() => root.Build();
    }
}