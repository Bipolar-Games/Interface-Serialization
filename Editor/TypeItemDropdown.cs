using System;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Bipolar.Editor
{
    internal abstract class TypeItemDropdown : AdvancedDropdown
    {
        public event Action<TypeItem> OnItemSelected;

        public Type SubcomponentType { get; private set; }

        private readonly AdvancedDropdownItem root;

        public TypeItemDropdown(Type requiredType, Type baseClass) : base(new AdvancedDropdownState())
        {
            SubcomponentType = requiredType;
            var types = TypeCache.GetTypesDerivedFrom(SubcomponentType)
                .Where(type => !type.IsAbstract && type.IsSubclassOf(baseClass));
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

        protected sealed override AdvancedDropdownItem BuildRoot() => root;

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
