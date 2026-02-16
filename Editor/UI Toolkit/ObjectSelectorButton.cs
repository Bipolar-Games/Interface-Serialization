#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bipolar.Editor
{
	public class ObjectSelectorButton : VisualElement
	{
		private InterfaceField objectField;
		private System.Type requiredType;

		public ObjectSelectorButton(InterfaceField objectField, System.Type requiredType)
		{
			this.objectField = objectField;
			this.requiredType = requiredType;
			AddToClassList(ObjectField.selectorUssClassName);
		}

		[EventInterest(new System.Type[] { typeof(MouseDownEvent) })]
		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			if (evt is MouseDownEvent mouseDownEvent && mouseDownEvent.button == 0)
			{
				InterfaceSelectorWindow.Show(requiredType, objectField.value, objectField.AssignValue);
			}
		}
	}
}
#endif
