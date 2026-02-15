#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Bipolar.Editor
{
	public class InterfaceField : ObjectField
	{
		public InterfaceField(string label, System.Type interfaceType) : base(label)
		{
			objectType = interfaceType;
			AddToClassList(alignedFieldUssClassName);
			this.Q(className: selectorUssClassName).style.display = DisplayStyle.None;

			style.flexGrow = 1;
			style.flexShrink = 1;
			style.minWidth = 0;

			var objectSelectorButton = new ObjectSelectorButton(this, interfaceType);
			this.Q(className: inputUssClassName).Add(objectSelectorButton);
		}
	}
}
#endif
