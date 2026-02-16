#if !BIPOLAR_DISABLE_UI_TOOLKIT
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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

		[EventInterest(typeof(KeyDownEvent))]
		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			if (evt == null)
				return;

			if (TryExecuteConfirmKeyboardAction(evt) == false)
				base.ExecuteDefaultActionAtTarget(evt);
		}

		private bool TryExecuteConfirmKeyboardAction(EventBase evt)
		{
			if (IsConfirmKeyboardEvent(evt) == false)
				return false;

			InterfaceSelectorWindow.Show(objectType, value, AssignValue);
			return true;
		}

		private static bool IsConfirmKeyboardEvent(EventBase evt)
		{
			return evt.eventTypeId == EventBase<KeyDownEvent>.TypeId()
				&& evt is KeyDownEvent keyDownEvent
				&& InterfaceEditorUtility.IsConfirmKey(keyDownEvent.keyCode);
		}

		internal void AssignValue(Object selected)
		{
			value = selected;
		}
	}
}
#endif
