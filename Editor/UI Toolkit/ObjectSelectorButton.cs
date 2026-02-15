using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bipolar.Editor
{
	public class ObjectSelectorButton : VisualElement
	{
		private BaseField<Object> objectField;
		private System.Type requiredType;

		public ObjectSelectorButton(BaseField<Object> objectField, System.Type requiredType)
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
				InterfaceSelectorWindow.Show(requiredType, objectField.value, AssignValue);
			}
		}

		private void AssignValue(Object selected)
		{
			objectField.value = selected;
		}
	}
}
