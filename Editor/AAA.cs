using System;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.Editor
{
    internal abstract class AAA : AdvancedDropdown
    {
        public event Action<TypeItem> OnItemSelected;

        public Type SubcomponentType { get; private set; }

        private readonly AdvancedDropdownItem root;

        public AAA(Type requiredType) : base(new AdvancedDropdownState())
        {
            SubcomponentType = requiredType;
            var types = TypeCache.GetTypesDerivedFrom(SubcomponentType)
                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(Component)));
            var builder = new TypeItemBuilder(GetRootItemName());
            PopulateBuilderWithTypes(builder, types);
            root = builder.Build();
        }

        protected abstract string GetRootItemName();

        private void PopulateBuilderWithTypes(TypeItemBuilder builder, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AddTypeToBuilder(builder, type);
            }
        }

        protected abstract void AddTypeToBuilder(TypeItemBuilder builder, Type type);

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
}
