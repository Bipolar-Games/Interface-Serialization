using System;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

namespace Bipolar.Editor
{
    internal class TypeItemBuilder
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